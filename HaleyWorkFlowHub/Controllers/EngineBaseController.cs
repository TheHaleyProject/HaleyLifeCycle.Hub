using Haley.Abstractions;
using Haley.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Haley.Models {

    // No class-level route here; let concrete controllers decide (e.g., [Route("api/[controller]")])
    public abstract class EngineBaseController : CommonWFController {
        public EngineBaseController(IWorkflowRepository wfRepository, ILoggerProvider logprovider) : base(wfRepository, logprovider.CreateLogger(nameof(EngineBaseController))) { }


        [HttpPost("register")]
        public virtual async Task<IActionResult> Register([FromQuery] string guid, [FromQuery] int env) {
            if (string.IsNullOrWhiteSpace(guid) || env < 1) return BadRequest(new Feedback(false, "Engine GUID and Environment are required."));
            if (!(guid.IsValidGuid(out _) && guid.IsCompactGuid(out _))) return BadRequest(new Feedback(false, "Guid is not in a valid format"));

            // Persist or upsert engine row (expects repo support)
            await _wfRepo.RegisterEngineAsync(guid,env);
            _logger.LogInformation("Engine {engineId} registered for env={env}", guid,env);

            return Ok(new Feedback(true, "Engine registered"));
        }

        [HttpPost("heartbeat")]
        public virtual async Task<IActionResult> Heartbeat([FromQuery] string guid) {
            if (string.IsNullOrWhiteSpace(guid)) return BadRequest(new Feedback(false, "EngineId is required."));
            if (!(guid.IsValidGuid(out _) && guid.IsCompactGuid(out _))) return BadRequest(new Feedback(false, "Guid is not in a valid format"));

            await _wfRepo.UpdateEngineHeartbeatAsync(guid);
            _logger.LogDebug("Heartbeat from {engineId}", guid);

            return Ok(new Feedback(true, "Heartbeat updated"));
        }

        [HttpGet("active")]
        public virtual async Task<IActionResult> GetActiveEngines() {
            var engines = await _wfRepo.LoadActiveEnginesAsync(1);
            return Ok(engines);
        }
    }
}
