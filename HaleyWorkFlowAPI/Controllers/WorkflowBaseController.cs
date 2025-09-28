using Haley.Abstractions;
using Haley.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Haley.Models.WorkflowConstants;

namespace Haley.Models {
    public abstract class WorkflowBaseController : CommonWFController {

        [HttpPost]
        public async Task<IFeedback> CreateWorkFlow([FromQuery] int code, [FromQuery] string? name ,[FromQuery] int? source) {
           return (await _wfRepo.CreateOrGetWorkflow(code,name,source ?? 0)).AsFeedBack();
        }

        WorkflowPayload PrepareWFPayload(object? data) {
            var payload = new WorkflowPayload();

            if (!string.IsNullOrWhiteSpace(data?.ToString()) && data.ToString().IsValidJson()) {
                var plDic = data.ToString().FromJson<Dictionary<string, object>>();
                if (plDic.TryGetValue(WFI_PAYLOAD.PARAMETERS, out var paramsObj) && paramsObj is Dictionary<string, object> paramDic) { payload.Parameters = paramDic; }
                if (plDic.TryGetValue(WFI_PAYLOAD.URLS, out var urlObj) && urlObj is Dictionary<string, string> urlDic) { payload.Urls = urlDic; }
                if (plDic.TryGetValue(WFI_PAYLOAD.SOURCE, out var sourceObj) && int.TryParse(sourceObj?.ToString(),out var instSource)) { payload.InstanceSource = instSource; }
                if (plDic.TryGetValue(WFI_PAYLOAD.REFERENCE, out var refObj) && !string.IsNullOrWhiteSpace(refObj?.ToString())) { payload.Reference = refObj?.ToString(); }
                if (plDic.TryGetValue(WFI_PAYLOAD.OWNER, out var ownObj) && long.TryParse(ownObj?.ToString(),out var ownerId)) { payload.Owner = ownerId; }
                //Engine ID is always the ID for this running application.
            }
            return payload;
        }

        [HttpPost]
        [Route("start/code")]
        public async Task<IFeedback> StartWorkflow([FromQuery] int code, [FromQuery] int? source, [FromBody] object? data = null) {
            var payload = PrepareWFPayload(data);
            return (await _wfEngine.StartWorkflow(code, source ?? 0, payload)).AsFeedBack();
        }

        [HttpPost]
        [Route("start/guid")]
        public async Task<IFeedback> StartWorkflow([FromQuery] Guid guid, [FromBody] object? data = null) {
            var payload = PrepareWFPayload(data);
            return (await _wfEngine.StartWorkflow(guid, payload)).AsFeedBack();
        }

        public WorkflowBaseController(IWorkflowRepository wfRepository, IWorkflowEngine wfEngine, ILoggerProvider logprovider) : base(wfRepository,wfEngine, logprovider.CreateLogger(nameof(WorkflowBaseController))) {
           
        }
    }
}
