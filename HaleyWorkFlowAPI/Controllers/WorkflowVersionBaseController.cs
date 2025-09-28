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
        public async Task<IFeedback> GetVersion([FromQuery] int wf_id) {
            return (await _wfRepo.GetWFVersion(wf_id));
        }

        [HttpGet]
        [Route("wf-code")]
        public async Task<IFeedback> GetVersionByWorkflowCode([FromQuery] int wf_code, [FromQuery] int? source) {
            return (await _wfRepo.GetWFVersion(wf_code, source ?? 0));
        }

        [HttpGet]
        [Route("guid")]
        public async Task<IFeedback> GetVersionByGuid([FromQuery] string guid) {
            Guid input = Guid.Empty;
            if (string.IsNullOrWhiteSpace(guid) || !(guid.IsValidGuid(out input) || guid.IsCompactGuid(out input))) return new Feedback(false, "Input is not a valid guid");
            return (await _wfRepo.GetWFVersion(input));
        }

        public WorkflowVersionBaseController(IWorkflowRepository wfRepository,  ILoggerProvider logprovider) : base(wfRepository, logprovider.CreateLogger(nameof(WorkflowVersionBaseController))) {
           
        }
    }
}
