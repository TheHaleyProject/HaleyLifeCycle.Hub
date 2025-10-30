using Haley.Abstractions;
using Haley.Enums;
using Haley.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static Haley.Abstractions.QueryFields;

namespace Haley.Utils {
    public class MariaDBRepository : IWorkflowRepository {
        private readonly IAdapterGateway _agw;

        public MariaDBRepository(IAdapterGateway gw) {
            _agw = gw;
            if (_agw == null) throw new ArgumentNullException($@"{nameof(IAdapterGateway)} cannot be empty for this operation.");
        }

        public async Task<IFeedback<Guid>> CreateVersionAsync(int workflowId, string definitionJson) {
            var fb = new Feedback<Guid>();
            try {
                if (workflowId < 1 || string.IsNullOrWhiteSpace(definitionJson) || !definitionJson.IsValidJson()) return fb.SetStatus(false).SetMessage("Workflow Id should be valid. Definition Json should be a valid JSON.");

                var wfExists = await _agw.Scalar(new AdapterArgs() { Query = QRY_WORKFLOW.EXISTS_BY_ID }, (ID, workflowId));
                if (wfExists == null) return fb.SetStatus(false).SetMessage($@"No workflow exists for the given id {workflowId}");

                //First get the latest version and check if the definition is the same or not.
                try {
                    var latestDefObj = await _agw.Read(new AdapterArgs() { Query = QRY_WF_VERSION.SELECT_DEF_BY_WF_ID, Filter = ResultFilter.FirstDictionary }, (WORKFLOW, workflowId));
                    if (latestDefObj != null && latestDefObj is Dictionary<string, object> latest && latest.TryGetValue("definition", out var latestDef)) {
                        //So some latest object exists. Check if the definition is same as the one we are trying to insert.
                        var hash1 = JsonSerializer.Serialize(JsonNode.Parse(latestDef.ToString()))?.ComputeHash();
                        var hash2 = JsonSerializer.Serialize(JsonNode.Parse(definitionJson))?.ComputeHash();
                        if (hash1 == hash2) {
                            Console.WriteLine($"Hash1: {hash1}");
                            Console.WriteLine($"Hash2: {hash2}");

                            if (latest.TryGetValue("guid", out var latestGuidObj) && Guid.TryParse(latestGuidObj?.ToString(), out var latestGuid)) return fb.SetStatus(true).SetMessage("Latest Version already contains the same definition json. Not creating a new version").SetResult(latestGuid);
                        }
                    }
                } catch (Exception ex) {
                    Console.WriteLine("Exception while trying to compare with latest version. Proceeding to create new version");
                }


                var versionObj = await _agw.Scalar(new AdapterArgs() { Query = QRY_WF_VERSION.NEXT_VERSION_NO }, (WORKFLOW, workflowId));
                if (versionObj == null || !int.TryParse(versionObj.ToString(), out int nextVersion)) return fb.SetStatus(false).SetMessage("Unable to calculate next version number.");

                var parameters = new Dictionary<string, object> { { WORKFLOW, workflowId }, { VERSION, nextVersion }, { DEFINITION, definitionJson } };
                Console.WriteLine($"Creating version {nextVersion} for workflow {workflowId}");

                var guidObj = await _agw.Scalar(parameters.ToAdapterArgs(QRY_WF_VERSION.INSERT));
                if (guidObj != null && Guid.TryParse(guidObj.ToString(), out Guid newGuid)) return fb.SetStatus(true).SetResult(newGuid);
                return fb.SetStatus(false).SetMessage("Failed to create workflow version.");
            } catch (Exception ex) {
                return fb.SetStatus(false).SetMessage(ex.Message);
            }
        }
        public async Task<IFeedback<Dictionary<string, object>>> CreateOrGetWorkflow(int code, string name, int source =0) {
            var fb = new Feedback<Dictionary<string, object>>();
            try {
                var parameters = new Dictionary<string, object> { { SOURCE, source }, { CODE, code }, { NAME, name } };
                var fetchFunc = async () =>
                {
                    var result = await _agw.Read(new AdapterArgs { Query = QRY_WORKFLOW.SELECT_BY_CODE, Filter = ResultFilter.FirstDictionary }, (SOURCE, source), (CODE, code));
                return result is Dictionary<string, object> dic && dic.Count > 0
                        ? fb.SetStatus(true).SetResult(dic).SetMessage(string.Empty)
                        : fb.SetStatus(false).SetMessage("Workflow not found.");
                };

                var existing = await fetchFunc();
                if (existing.Status) return existing.SetMessage("Already Exists");

                if (string.IsNullOrWhiteSpace(name)) parameters[NAME] = $"WORKFLOW_{source}_{code}";
                await _agw.NonQuery(parameters.ToAdapterArgs(QRY_WORKFLOW.INSERT));

                return await fetchFunc();
            } catch (Exception ex) {
                return fb.SetStatus(false).SetMessage(ex.Message);
            }
        }
        public async Task UpdateWorkflow( int code, string name, int source =0) {
            var affected = await _agw.NonQuery(
                new AdapterArgs { Query = QRY_WORKFLOW.UPDATE },
                (SOURCE, source),
                (CODE, code),
                (NAME, name)
            );
        }
        public async Task DeleteWorkflowAsync(int code) {
            await _agw.NonQuery(new AdapterArgs() { Query = QRY_WORKFLOW.DELETE }, (CODE, code));
        }

        public async Task<IFeedback<Dictionary<string, object>>> CreateOrGetAppSourceAsync(int code, string name) {
            var fb = new Feedback<Dictionary<string, object>>();
            try {
                var parameters = new Dictionary<string, object> { { CODE, code }, { NAME, name } };

                var sourceFunc = async () => {
                    var idObj = await _agw.Read(new AdapterArgs() { Query = QRY_APP_SOURCE.SELECT_BY_CODE, Filter = ResultFilter.FirstDictionary }, (CODE, code));
                    if (idObj != null && idObj is Dictionary<string, object> dic1 && dic1.Count > 0) return fb.SetStatus(true).SetMessage(string.Empty).SetResult(dic1);
                    return fb.SetStatus(false).SetMessage("Unable to create or fetch any appsource"); // Since CODE is the PK, we return it directly
                };

                var sourceCheck = await sourceFunc();
                if (sourceCheck != null && sourceCheck.Status) return sourceCheck.SetMessage("Already Exists");

                //For creating name is required.
                if (string.IsNullOrWhiteSpace(name)) parameters[NAME] = $@"APPSOURCE_{code}";
                await _agw.NonQuery(parameters.ToAdapterArgs(QRY_APP_SOURCE.INSERT));
                
                return await sourceFunc();
            } catch (Exception ex) {
                return fb.SetStatus(false).SetMessage(ex.Message);
            }
        }

        async Task<IFeedback<WorkflowDefinition>> LoadWorkflowInternal(Dictionary<string, object> dic) {
            var fb = new Feedback<WorkflowDefinition>();
            try {
                if (dic == null || !dic.TryGetValue("definition", out var defJsonObj))  return fb.SetMessage($"Unable to load workflow definition for the given inputs");

                var defJson = defJsonObj?.ToString();
                if (string.IsNullOrWhiteSpace(defJson) || !defJson.IsValidJson()) return fb.SetMessage("Definition JSON is missing or invalid.");

                var def = defJson.FromJson<WorkflowDefinition>();
                if (def == null) return fb.SetMessage("Failed to deserialize workflow definition.");

                // Hydrate metadata from DB
                def.Id = Convert.ToInt32(dic["workflow"]);
                def.Key = dic["code"]?.ToString();
                def.Name = dic["name"]?.ToString();
                def.Version = Convert.ToInt32(dic["version"]);
                def.SetGuid(Guid.Parse(dic["guid"].ToString()));

                Console.WriteLine($"Deserialized workflow: {def.Name} (v{def.Version}) [{def.Guid}]");
                return fb.SetStatus(true).SetResult(def);
            } catch (Exception ex) {
                return fb.SetStatus(false).SetMessage(ex.Message);
            }
        }

        public async Task<IFeedback<WorkflowDefinition>> LoadWorkflow(Guid guid) {
            var defFb = await GetWFVersion(guid); // assumes source = 0 for global
            if (!defFb.Status || defFb.Result is not Dictionary<string, object> dic || !dic.TryGetValue("definition", out var defJsonObj))
                return new Feedback<WorkflowDefinition>(false, $"Unable to load workflow definition for the given guid {guid}");
            return await LoadWorkflowInternal(dic);
        }

        public async Task<IFeedback<WorkflowDefinition>> LoadWorkflow(int code, int source) {
            var defFb = await GetWFVersion(code,source); // assumes source = 0 for global
            if (!defFb.Status || defFb.Result is not Dictionary<string, object> dic || !dic.TryGetValue("definition", out var defJsonObj))
                return new Feedback<WorkflowDefinition>(false, $"Unable to load workflow definition for the given code {code} and source {source}");
            return await LoadWorkflowInternal(dic);
        }

        public async Task<IFeedback<Guid>> GetGuidByWfCode(int code, int source = 0) {
            var fb = new Feedback<Guid>();
            if (code < 1) return fb.SetStatus(false).SetMessage("Invalid workflow code.");

            var result = await _agw.Scalar(
                new AdapterArgs { Query = QRY_WORKFLOW.GET_GUID_BY_CODE},
                (SOURCE, source), // Assuming global workflows
                (CODE, code)
            );

            if (result != null && Guid.TryParse(result.ToString(),out var guid)) return fb.SetStatus(true).SetResult(guid);
            return fb.SetStatus(false).SetMessage($"Unable to fetch the Guid for the given inputs, code {code} and source {source}.");
        }

        public async Task<int> GetVersionIdByDefinitionGuidAsync(Guid versionGuid) {
            if (versionGuid == Guid.Empty) return 0;
            var result = await _agw.Scalar(new AdapterArgs { Query = QRY_WF_VERSION.SELECT_ID_BY_GUID },(GUID, versionGuid));
            return result != null && int.TryParse(result.ToString(), out int id) ? id : 0;
        }

        public async Task<IFeedback<long>> SaveInstanceAsync(WorkflowInstance instance) {
            var fb = new Feedback<long>();

            //Definition is the workflow definition based on which the instance is generated.
            if (instance == null || instance.DefinitionId == Guid.Empty || instance.Environment < 1) //Environment should be a valid id.
                return fb.SetStatus(false).SetMessage("Invalid workflow instance input.");

            var guid = instance.Guid == Guid.Empty ? Guid.NewGuid() : instance.Guid;
            instance.SetGuid(guid); // Ensure it's set on the object

            //First Check if the environment is availalbe or not
            var envExists = await _agw.Scalar(new AdapterArgs() { Query = QRY_ENVIRONMENT.EXISTS_BY_CODE }, (CODE, instance.Environment));
            if (envExists == null || !int.TryParse(envExists?.ToString(), out _)) return fb.SetStatus(false).SetMessage($@"Environment doesn't exists for the given code {instance.Environment}");

            var parameters = new Dictionary<string, object> {
                    { "@GUID", guid },
                    { "@WF_VERSION", await GetVersionIdByDefinitionGuidAsync(instance.DefinitionId) },
                    { "@ENV", instance.Environment },
                    { "@LOCKED_BY", instance.Owner }, //Who is locki
                    { "@STATUS", (int)instance.State }
                };

            var result = await _agw.Scalar(parameters.ToAdapterArgs(QRY_WFINSTANCE.INSERT_INSTANCE));
            if (result == null || !long.TryParse(result.ToString(), out long newId))
                return fb.SetStatus(false).SetMessage("Failed to insert wf_instance.");

            instance.Id = newId;

            // Save wf_info
            await _agw.NonQuery(new Dictionary<string, object>
            {
        { "@WFI", newId },
        { "@PARAMETERS", JsonSerializer.Serialize(instance.Parameters ?? new()) },
        { "@URL_OVERRIDES", JsonSerializer.Serialize(instance.Urls ?? new()) }
    }.ToAdapterArgs(QRY_WFINSTANCE.INSERT_INFO));

            // Save wfi_belongs_to (if Reference is present)
            if (!string.IsNullOrWhiteSpace(instance.Reference)) {
                await _agw.NonQuery(new Dictionary<string, object>
                {
            { "@WFI_ID", newId },
            { "@SOURCE", instance.Owner },
            { "@OWNER", instance.Owner },
            { "@REF", instance.Reference }
        }.ToAdapterArgs(QRY_WFINSTANCE.INSERT_BELONGS_TO));
            }

            return fb.SetStatus(true).SetResult(newId);
        }

        public async Task<IFeedback> GetWFVersion(int workflowId) {
            var fb = new Feedback();

            if (workflowId < 1) return fb.SetStatus(false).SetMessage("Invalid workflow ID.");

            var result = await _agw.Read(new AdapterArgs { Query = QRY_WF_VERSION.SELECT_DEF_BY_WF_ID, Filter = ResultFilter.FirstDictionary, JsonStringAsNode = true }, (WORKFLOW, workflowId));
            if (result is Dictionary<string, object> dic && dic.Count > 0)
                return fb.SetStatus(true).SetResult(dic);

            return fb.SetStatus(false).SetMessage($"No version found for workflow ID {workflowId}.");
        }


        public async Task<IFeedback> GetWFVersion(Guid guid) {
            var fb = new Feedback();

            if (guid == Guid.Empty) return fb.SetStatus(false).SetMessage("Invalid version GUID.");

            var result = await _agw.Read(new AdapterArgs { Query = QRY_WF_VERSION.SELECT_DEF_BY_GUID, Filter = ResultFilter.FirstDictionary, JsonStringAsNode=true }, (GUID, guid));
            if (result is Dictionary<string, object> dic && dic.Count > 0) return fb.SetStatus(true).SetResult(dic);
            if (result is IFeedback sqlfb) return sqlfb;
            return fb.SetStatus(false).SetMessage($"No version found for GUID {guid}.");
        }

        public async Task<IFeedback> GetWFVersion(int wf_code, int source = 0) {
            var fb = new Feedback();

            if (wf_code < 1) return fb.SetStatus(false).SetMessage("Invalid workflow code.");

            var result = await _agw.Read(
                new AdapterArgs { Query = QRY_WF_VERSION.SELECT_DEF_BY_WF_CODE, Filter = ResultFilter.FirstDictionary, JsonStringAsNode = true },
                (SOURCE, source), // Assuming global workflows
                (CODE, wf_code)
            );

            if (result is Dictionary<string, object> dic && dic.Count > 0) return fb.SetStatus(true).SetResult(dic);

            return fb.SetStatus(false).SetMessage($"No version found for workflow code {wf_code} and source {source}.");
        }

        public Task<WorkflowDefinition> LoadWorkflowAsync(int id) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WorkflowDefinition>> LoadAllWorkflowsAsync() {
            throw new NotImplementedException();
        }

        public Task<WorkflowDefinition> LoadVersionAsync(Guid versionGuid) {
            throw new NotImplementedException();
        }

        public Task<WorkflowDefinition> LoadLatestVersionAsync(int workflowId) {
            throw new NotImplementedException();
        }

        public Task MarkVersionAsPublishedAsync(Guid versionGuid) {
            throw new NotImplementedException();
        }

        Task IWorkflowRepository.SaveInstanceAsync(WorkflowInstance instance) {
            return SaveInstanceAsync(instance);
        }

        public Task<WorkflowInstance> LoadInstanceAsync(Guid instanceGuid) {
            throw new NotImplementedException();
        }

        public Task UpdateInstanceAsync(WorkflowInstance instance) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WorkflowInstance>> LoadPendingInstancesAsync(string environment, int limit = 10) {
            throw new NotImplementedException();
        }

        public Task<bool> ClaimInstanceAsync(Guid instanceGuid, int engineId) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WorkflowInstance>> LoadOrphanedInstancesAsync(string environment) {
            throw new NotImplementedException();
        }

        public Task<WorkflowState> LoadStateAsync(Guid instanceGuid) {
            throw new NotImplementedException();
        }

        public Task UpdateStateAsync(Guid instanceGuid, WorkflowState state) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WorkflowStep>> LoadStepsAsync(Guid instanceGuid) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StepLog>> LoadLogsAsync(Guid instanceGuid) {
            throw new NotImplementedException();
        }

        public Task AddStepLogAsync(StepLog log) {
            throw new NotImplementedException();
        }

        public async Task RegisterEngineAsync(string guid, int environment) {
            var parameters = new Dictionary<string, object> {
                [GUID] = guid,
                [ENVIRONMENT] = environment,
                [STATUS] = 1,
                [LAST_BEAT] = DateTime.UtcNow
            };

            await _agw.NonQuery(parameters.ToAdapterArgs(QRY_ENGINE.INSERT));
        }

        public async Task UpdateEngineHeartbeatAsync(string guid) {
            await _agw.NonQuery(
                new AdapterArgs { Query = QRY_ENGINE.UPDATE_BEAT_BY_GUID },
                (GUID, guid),
                (LAST_BEAT, DateTime.UtcNow)
            );
        }

        public async Task<IEnumerable<WorkflowEngineEntity>> LoadActiveEnginesAsync(int environment, TimeSpan? heartbeatThreshold = null) {
            var cutoff = DateTime.UtcNow - (heartbeatThreshold ?? TimeSpan.FromMinutes(2));

            var result = await _agw.Read(
                new AdapterArgs { Query = QRY_ENGINE.SELECT_BY_ENVIRONMENT },
                (ENVIRONMENT, environment)
            );

            // You can filter by cutoff in SQL if you extend QRY_ENGINE, or filter here in C#
            return (result as IEnumerable<Dictionary<string, object>> ?? new List<Dictionary<string, object>>())
                .Select(dic => new WorkflowEngineEntity {
                    Id = Convert.ToInt32(dic["id"]),
                    Guid = dic["guid"].ToString(),
                    Environment = Convert.ToInt32(dic["environment"]),
                    LastBeat = Convert.ToDateTime(dic["last_beat"]),
                    Status = Convert.ToInt32(dic["status"])
                })
                .Where(e => e.LastBeat >= cutoff && e.Status == 1);
        }

        public async Task<WorkflowEngineEntity?> LoadEngineByGuidAsync(string guid) {
            var result = await _agw.Read(
                new AdapterArgs { Query = QRY_ENGINE.SELECT_BY_GUID, Filter = ResultFilter.FirstDictionary },
                (GUID, guid)
            );

            if (result is Dictionary<string, object> dic && dic.Count > 0) {
                return new WorkflowEngineEntity {
                    Id = Convert.ToInt32(dic["id"]),
                    Guid = dic["guid"].ToString(),
                    Environment = Convert.ToInt32(dic["environment"]),
                    LastBeat = Convert.ToDateTime(dic["last_beat"]),
                    Status = Convert.ToInt32(dic["status"])
                };
            }
            return null;
        }

        public async Task RetireEngineAsync(string guid) {
            await _agw.NonQuery(
                new AdapterArgs { Query = QRY_ENGINE.MARK_DEAD_BY_GUID },
                (GUID, guid)
            );
        }


    }
}
