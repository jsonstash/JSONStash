using JSONStash.Common.Context;
using JSONStash.Web.Service.Attributes;
using JSONStash.Web.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HobbyTail.Web.Service.Controllers
{
    [Authorize]
    [Route("i")]
    [ApiController]
    [ApiVersion("1.0")]
    public class InfoController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly JSONStashContext _context;

        public InfoController(JSONStashContext context, IConfiguration configuration, ILogger<InfoController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        [Produces("application/json")]
        public IActionResult GetApiInfo() => base.Json(new { ApiInfo.Name, ApiInfo.Description, ApiInfo.Version});
    }
}
