using Haley.Abstractions;
using Haley.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Haley.Models {
    public abstract class WorkflowBaseController : CommonWFController {

        [HttpPost]
        public async Task<IFeedback> CreateWorkFlow([FromQuery] int code, [FromQuery] string? name ,[FromQuery] int? source) {
           return (await _wfRepo.CreateOrGetWorkflowAsync(code,name,source ?? 0)).AsFeedBack();
        }

        public WorkflowBaseController(IWorkflowRepository wfRepository, IWorkflowEngine wfEngine, ILoggerProvider logprovider) : base(wfRepository,wfEngine, logprovider.CreateLogger(nameof(WorkflowBaseController))) {
           
        }
    }
}
