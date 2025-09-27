using Haley.Abstractions;
using Haley.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Haley.Models {
    public abstract class APISourceBaseController : CommonWFController {

        [HttpPost]
        public async Task<IFeedback> CreateAppSource([FromQuery] int code,[FromQuery] string? name= null) {
           return (await _wfRepo.CreateOrGetAppSourceAsync(code, name)).AsFeedBack();
        }

        public APISourceBaseController(IWorkflowRepository wfRepository, IWorkflowEngine wfEngine, ILoggerProvider logprovider) : base(wfRepository,wfEngine, logprovider.CreateLogger(nameof(APISourceBaseController))) {
           
        }
    }
}
