using JSONStash.Common.Context;
using JSONStash.Common.Models;
using JSONStash.Common.Services.IServices;
using JSONStash.Web.Service.Attributes;
using JSONStash.Web.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace JSONStash.Web.Service.Controllers
{
    [Route("s")]
    [ApiController]
    [ApiVersion("1.0")]
    public class StashController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IJSONStashService _service;

        public StashController(IJSONStashService service, IConfiguration configuration, ILogger<StashController> logger)
        {
            _service = service;
            _configuration = configuration;
            _logger = logger;
        }


        /// <summary>
        /// List stashes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [JWTAuthorize]
        [Produces("application/json")]
        public async Task<IActionResult> GetStashes()
        {
            try
            {
                User user = (User)HttpContext.Items["User"];

                StashMetadata[] stashes = _service.GetStashes(user);

                return Ok(stashes);
            }
            catch
            {
                return BadRequest("There was an issue with your request. Please, contact the administrator.");
            }
        }

        /// <summary>
        /// Get stash data.
        /// </summary>
        /// <param name="stashId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{stashId}")]
        [KeyAuthorize]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> GetStashData(string stashId)
        {
            try
            {
                User user = (User)HttpContext.Items["User"];

                bool isValidStashId = Guid.TryParse(stashId, out Guid stashGuid);

                if (isValidStashId)
                {
                    StashData stashData = await _service.GetStash(stashGuid);

                    return Ok(stashData);
                }
                else
                {
                    return BadRequest("Bad stash id. Please, refer to the api documentation on getting a stash.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the get stash action in stash controller.");

                return BadRequest("There was an issue with your request. Please, contact the administrator.");
            }
        }


        /// <summary>
        /// Create stash.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [JWTAuthorize]
        [RecordSizeLimit]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Create(object json)
        {
            try
            {
                JObject data = JObject.FromObject(json);

                User user = (User)HttpContext.Items["User"];

                bool hasName = HttpContext.Request.Headers.TryGetValue("x-stash-name", out StringValues name);

                HttpContext.Request.Headers.TryGetValue("x-collection-id", out StringValues collection);

                if (hasName && data != null)
                {
                    int.TryParse(_configuration["StashNameMaxLength"], out int stashNameMaxLength);

                    if (name[0].Length > stashNameMaxLength)
                        return BadRequest("Stash name is too long. Please, refer to the api documentation on stash name length.");

                    Guid? collectionGuid = Guid.TryParse(collection.ToString(), out Guid temp) ? temp : null;

                    StashMetadata stashMetadata = await _service.CreateStash(user, name, data, collectionGuid);

                    return Ok(stashMetadata);
                }
                else
                {
                    return BadRequest("Missing stash name. Please, refer to the api documentation on creating a stash.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the create stash action in stash controller.");

                return BadRequest("There was an issue with your request. Please, contact the administrator.");
            }
        }


        /// <summary>
        /// Update name of stash.
        /// </summary>
        /// <param name="stashId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{stashId}/name")]
        [JWTAuthorize]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateName(string stashId)
        {
            try
            {
                User user = (User)HttpContext.Items["User"];

                bool hasName = HttpContext.Request.Headers.TryGetValue("x-stash-name", out StringValues name);

                bool isValidStashId = Guid.TryParse(stashId, out Guid stashGuid);

                if (hasName && isValidStashId)
                {
                    int.TryParse(_configuration["StashNameMaxLength"], out int stashNameMaxLength);

                    if (name[0].Length > stashNameMaxLength)
                        return BadRequest("Stash name is too long. Please, refer to the api documentation on stash name length.");

                    bool updated = await _service.UpdateStashName(user, stashGuid, name);

                    return updated ? Ok("Stash name was updated.") : BadRequest("There was an issue with your request. Please, contact the administrator."); ;
                }
                else
                {
                    return BadRequest("Missing new stash name or bad stash id. Please, refer to the api documentation on updating a stash name.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the update stash name action in stash controller.");

                return BadRequest("There was an issue with your request. Please, contact the administrator.");
            }
        }

        /// <summary>
        /// Update stash data.
        /// </summary>
        /// <param name="stashId"></param>
        /// <returns></returns>
        [HttpPut]
        [RecordSizeLimit]
        [Route("{stashId}")]
        [KeyAuthorize]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateStashData(string stashId, object json)
        {
            try
            {
                JObject record = JObject.FromObject(json);

                bool isValidStashId = Guid.TryParse(stashId, out Guid stashGuid);

                if (isValidStashId)
                {
                    StashData stashData = await _service.UpdateStashData(stashGuid, record);

                    return Ok(stashData);
                }
                else
                {
                    return BadRequest("Bad stash id. Please, refer to the api documentation on updating a stash.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the update stash action in stash controller.");

                return BadRequest("There was an issue with your request. Please, contact the administrator.");
            }
        }

        /// <summary>
        /// Delete stash.
        /// </summary>
        /// <param name="stashId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{stashId}")]
        [JWTAuthorize]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Delete(string stashId)
        {
            try
            {
                User user = (User)HttpContext.Items["User"];

                bool isValidStashId = Guid.TryParse(stashId, out Guid stashGuid);

                if (isValidStashId)
                {
                    bool deleted = await _service.DeleteStash(user, stashGuid);

                    return deleted ? Ok("Stash has been deleted.") : BadRequest("There was an issue with your request. Please, contact the administrator."); ;
                }
                else
                {
                    return BadRequest("Bad stash id. Please, refer to the api documentation on adding a record to a stash.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the delete stash action in stash controller.");

                return BadRequest("There was an issue with your request. Please, contact the administrator.");
            }
        }
    }
}
