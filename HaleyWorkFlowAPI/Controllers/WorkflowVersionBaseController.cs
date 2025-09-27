using Haley.Abstractions;
using Haley.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Haley.Models {
    public abstract class WorkflowVersionBaseController : CommonWFController {

        [HttpPost]
        public async Task<IFeedback> CreateVersion([FromQuery] int workflow_id,[FromBody] object wf_defition) {
           return (await _wfRepo.CreateVersionAsync(workflow_id, wf_defition?.ToString())).AsFeedBack();
        }

        [HttpGet]
        public async Task<IFeedback> GetVersion([FromQuery] int workflow_id) {
            return (await _wfRepo.GetVersionAsync(workflow_id));
        }

        [HttpGet]
        [Route("guid")]
        public async Task<IFeedback> GetVersionByGuid([FromQuery] string guid) {
            Guid input = Guid.Empty;
            if (string.IsNullOrWhiteSpace(guid) || !(guid.IsValidGuid(out input) || guid.IsCompactGuid(out input))) return new Feedback(false, "Input is not a valid guid");
            return (await _wfRepo.GetVersionByGUIDAsync(input));
        }

        public WorkflowVersionBaseController(IWorkflowRepository wfRepository, IWorkflowEngine wfEngine, ILoggerProvider logprovider) : base(wfRepository,wfEngine, logprovider.CreateLogger(nameof(WorkflowVersionBaseController))) {
           
        }
    }
}
