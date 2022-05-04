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
    [Authorize]
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
        [Produces("application/json")]
        public async Task<IActionResult> GetStashes()
        {
            try
            {
                User user = (User)HttpContext.Items["User"];

                Metadata[] stashes = _service.GetStashes(user);

                return Ok(stashes);
            }
            catch
            {
                return BadRequest("There was an issue with your request. Please, contact the administrator.");
            }
        }

        /// <summary>
        /// Get record from stash.
        /// </summary>
        /// <param name="stashId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{stashId}")]
        [Route("{stashId}/latest")]
        [Route("{stashId}/{version}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> GetRecord(string stashId, int? version)
        {
            try
            {
                User user = (User)HttpContext.Items["User"];

                bool isValidStashId = Guid.TryParse(stashId, out Guid stashGuid);

                if (isValidStashId)
                {
                    StashRecord stashRecord = await _service.GetStashRecord(user, stashGuid, version);

                    return Ok(stashRecord);
                }
                else
                {
                    return BadRequest("Bad stash id or version does not exist. Please, refer to the api documentation on getting a stash record.");
                }
            }
            catch
            {
                return BadRequest("There was an issue with your request. Please, contact the administrator.");
            }
        }

        /// <summary>
        /// Get stash version count.
        /// </summary>
        /// <param name="stashId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{stashId}/metadata")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> GetMetadata(string stashId)
        {
            try
            {
                User user = (User)HttpContext.Items["User"];

                bool isValidStashId = Guid.TryParse(stashId, out Guid stashGuid);

                if (isValidStashId)
                {
                    Metadata metadata = await _service.GetStashMetadata(user, stashGuid);

                    return Ok(metadata);
                }
                else
                {
                    return BadRequest("Bad stash id. Please, refer to the api documentation on getting stash versions.");
                }
            }
            catch
            {
                return BadRequest("There was an issue with your request. Please, contact the administrator.");
            }
        }

        /// <summary>
        /// Create stash and add first record.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [RecordSizeLimit]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Create(object json)
        {
            try
            {
                JObject record = JObject.FromObject(json);

                User user = (User)HttpContext.Items["User"];

                bool hasName = HttpContext.Request.Headers.TryGetValue("x-stash-name", out StringValues name);

                HttpContext.Request.Headers.TryGetValue("x-collection-id", out StringValues collection);

                if (hasName && record != null)
                {
                    int.TryParse(_configuration["StashNameMaxLength"], out int stashNameMaxLength);

                    if (name[0].Length > stashNameMaxLength)
                        return BadRequest("Stash name is too long. Please, refer to the api documentation on stash name length.");

                    Guid? collectionGuid = Guid.TryParse(collection.ToString(), out Guid temp) ? temp : null;

                    StashRecord stashRecord = await _service.CreateStash(user, name, record, collectionGuid);

                    return Ok(stashRecord);
                }
                else
                {
                    return BadRequest("Missing stash name. Please, refer to the api documentation on creating a stash.");
                }
            }
            catch
            {
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
            catch
            {
                return BadRequest("There was an issue with your request. Please, contact the administrator.");
            }
        }

        /// <summary>
        /// Add record version to stash.
        /// </summary>
        /// <param name="stashId"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{stashId}")]
        [RecordSizeLimit]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> AddRecordVersion(string stashId, object json)
        {
            try
            {
                User user = (User)HttpContext.Items["User"];

                JObject record = JObject.FromObject(json);

                bool isValidStashId = Guid.TryParse(stashId, out Guid stashGuid);

                if (isValidStashId)
                {
                    StashRecord stashRecord = await _service.AddStashRecordVersion(user, stashGuid, record);

                    return Ok(stashRecord);
                }
                else
                {
                    return BadRequest("Bad stash id. Please, refer to the api documentation on adding a record to a stash.");
                }
            }
            catch
            {
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
            catch
            {
                return BadRequest("There was an issue with your request. Please, contact the administrator.");
            }
        }
    }
}
