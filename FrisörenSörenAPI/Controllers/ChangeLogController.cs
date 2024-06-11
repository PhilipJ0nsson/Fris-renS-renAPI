using FrisörenSörenAPI.Dto;
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
        public async Task<ActionResult<IEnumerable<ChangeLogDto>>> GetChangelog()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("User not logged in.");
            }
            if (userRole != "Company")
            {
                return Unauthorized("User does not have access.");
            }
            var changelog = await _changelogService.GetChangelogAsync();
            var changelogDto = changelog.Select(c => new ChangeLogDto { Details = c.Details }).ToList();
            return Ok(changelogDto);
        }
    }
}
