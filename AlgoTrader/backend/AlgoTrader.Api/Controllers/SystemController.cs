using AlgoTrader.Core.Risk.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace AlgoTrader.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController(KillSwitch killSwitch) : ControllerBase
    {
        private readonly KillSwitch killSwitch = killSwitch;

        [HttpPost("kill")]
        public IActionResult Kill()
        {
            killSwitch.Trigger();

            return Ok("Kill switch activated. All trading has been halted");
        }

        [HttpGet("getHealth")]
        public IActionResult GetHealth()
        {
            return Ok(new { Status = "Running", KillSwitch = killSwitch.IsTriggered ? "Triggered" : "Active" });
        }
    }
}
