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

        public WorkflowVersionBaseController(IWorkflowRepository wfRepository, IWorkflowEngine wfEngine, ILoggerProvider logprovider) : base(wfRepository,wfEngine, logprovider.CreateLogger(nameof(WorkflowVersionBaseController))) {
           
        }
    }
}
