using Haley.Abstractions;
using Haley.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Haley.Models {

    // No class-level route here; let concrete controllers decide (e.g., [Route("api/[controller]")])
    public abstract class EngineBaseController : CommonWFController {
        public EngineBaseController(IWorkflowRepository wfRepository, ILoggerProvider logprovider) : base(wfRepository, logprovider.CreateLogger(nameof(EngineBaseController))) { }


        [HttpPost("register")]
        public virtual async Task<IActionResult> Register([FromBody] EngineRegistrationRequest request) {
            if (string.IsNullOrWhiteSpace(request.EngineId) || request.Environment < 1)
                return BadRequest(new Feedback(false, "EngineId and Environment are required."));

            // Persist or upsert engine row (expects repo support)
            await _wfRepo.RegisterEngineAsync(request.EngineId, request.Environment);
            _logger.LogInformation("Engine {engineId} registered for env={env}", request.EngineId, request.Environment);

            return Ok(new Feedback(true, "Engine registered"));
        }

        [HttpPost("heartbeat")]
        public virtual async Task<IActionResult> Heartbeat([FromBody] EngineHeartbeatRequest request) {
            if (string.IsNullOrWhiteSpace(request.EngineId))
                return BadRequest(new Feedback(false, "EngineId is required."));

            await _wfRepo.UpdateEngineHeartbeatAsync(request.EngineId, request.Timestamp == default ? DateTime.UtcNow : request.Timestamp);
            _logger.LogDebug("Heartbeat from {engineId} at {ts}", request.EngineId, request.Timestamp);

            return Ok(new Feedback(true, "Heartbeat updated"));
        }

        [HttpGet("active")]
        public virtual async Task<IActionResult> GetActiveEngines() {
            var engines = await _wfRepo.LoadActiveEnginesAsync(1);
            return Ok(engines);
        }
    }
}
