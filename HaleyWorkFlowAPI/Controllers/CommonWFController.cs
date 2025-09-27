using Haley.Abstractions;
using Haley.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Haley.Models {
    public abstract class CommonWFController : ControllerBase {
        protected IWorkflowRepository _wfRepo;
        protected IWorkflowEngine _wfEngine;
        protected ILogger _logger;

        public CommonWFController(IWorkflowRepository wfRepository, IWorkflowEngine wfEngine, ILogger logger) {
            //Do operations with both repository and also with engine
            _wfRepo = wfRepository;
            _wfEngine = wfEngine;
            _logger = logger;
        }
    }
}
