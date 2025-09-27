using Haley.Abstractions;
using Haley.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Haley.Models {
    public abstract class WorkflowBaseController : CommonWFController {

        [HttpPost]
        public async Task<IFeedback> CreateWorkFlow([FromQuery] int code, [FromQuery] string? name ,[FromQuery] int? source) {
           return (await _wfRepo.CreateOrGetWorkflow(code,name,source ?? 0)).AsFeedBack();
        }

        [HttpPost]
        [Route("start/code")]
        public async Task<IFeedback> StartWorkflow([FromQuery] int code, [FromQuery] int? source, [FromBody] Dictionary<string,object>? payload) {

            Dictionary<string, object> parameters = null;
            Dictionary<string, string> urls = null;

            if (payload != null && payload.TryGetValue("params", out var paramsObj) && paramsObj is Dictionary<string, object> paramDic) { parameters = paramDic; }
            if (payload != null && payload.TryGetValue("urls", out var urlObj) && urlObj is Dictionary<string, string> urlDic) { urls = urlDic; }
            return (await _wfEngine.StartWorkflow(code, source ?? 0, parameters ?? null ,urls ?? null)).AsFeedBack();
        }

        [HttpPost]
        [Route("start/guid")]
        public async Task<IFeedback> StartWorkflow([FromQuery] Guid guid, [FromBody] Dictionary<string, object>? payload) {
            Dictionary<string, object> parameters = null;
            Dictionary<string, string> urls = null;

            if (payload != null && payload.TryGetValue("params", out var paramsObj) && paramsObj is Dictionary<string, object> paramDic) { parameters = paramDic; }
            if (payload != null && payload.TryGetValue("urls", out var urlObj) && urlObj is Dictionary<string, string> urlDic) { urls = urlDic; }
            return (await _wfEngine.StartWorkflow(guid, parameters ?? null, urls ?? null)).AsFeedBack();
        }

        public WorkflowBaseController(IWorkflowRepository wfRepository, IWorkflowEngine wfEngine, ILoggerProvider logprovider) : base(wfRepository,wfEngine, logprovider.CreateLogger(nameof(WorkflowBaseController))) {
           
        }
    }
}
