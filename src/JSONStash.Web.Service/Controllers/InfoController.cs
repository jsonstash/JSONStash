﻿using JSONStash.Common.Context;
using JSONStash.Web.Service.Attributes;
using JSONStash.Web.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HobbyTail.Web.Service.Controllers
{
    [JWTAuthorize]
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

        /// <summary>
        /// Returns the current informational properties of the api.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [Produces("application/json")]
        public IActionResult GetApiInfo()
        {
            try
            {
                return Json(new { ApiInfo.Name, ApiInfo.Description, ApiInfo.Version });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the get api info action in info controller.");

                return BadRequest("There was an issue with your request. Please, contact the administrator.");
            }
        }
    }
}
