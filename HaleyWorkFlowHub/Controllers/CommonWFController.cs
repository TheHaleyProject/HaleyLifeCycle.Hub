using Haley.Abstractions;
using Haley.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Haley.Models {
    public abstract class CommonWFController : ControllerBase {
        protected IWorkflowRepository _wfRepo;
            protected ILogger _logger;

        public CommonWFController(IWorkflowRepository wfRepository, ILogger logger) {
            //Do operations with both repository and also with engine
            _wfRepo = wfRepository;
            _logger = logger;
        }
    }
}
