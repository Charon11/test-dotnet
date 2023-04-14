using Microsoft.AspNetCore.Mvc;
using Serilog;
using Todo_dotnet.Services;

namespace Todo_dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GHController : ControllerBase
    {
        private readonly IGithubService _service;

        public GHController(IGithubService service)
        {
            _service = service;
        }
        
        [HttpGet("branches")]
        public async Task<ActionResult<IList<dynamic>>> GetBranch()
        {
            Log.Information("Call Github to get branches list");
            var ghBranch = await _service.GetGhBranch();

            if (ghBranch == null)
            {
                return NotFound();
            }

            return Ok(ghBranch);
        }
    }
}
