using Microsoft.AspNetCore.Mvc;
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
        
        [HttpGet("ap")]
        public async Task<ActionResult<string>> GetBranch()
        {

            var ghBranch = await _service.getGHBranch();

            if (ghBranch == null)
            {
                return NotFound();
            }

            return ghBranch;
        }
    }
}
