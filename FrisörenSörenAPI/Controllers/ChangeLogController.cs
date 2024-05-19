using FrisörenSörenAPI.Interfaces;
using FrisörenSörenModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrisörenSörenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangeLogController : ControllerBase
    {
        private readonly IChangeLogService _changelogService;

        public ChangeLogController(IChangeLogService changelogService)
        {
            _changelogService = changelogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChangeLog>>> GetChangelog()
        {
            var changelog = await _changelogService.GetChangelogAsync();
            return Ok(changelog);
        }
    }
}
