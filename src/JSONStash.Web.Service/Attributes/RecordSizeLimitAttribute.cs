using JSONStash.Common.Models;
using JSONStash.Web.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JSONStash.Web.Service.Attributes
{
    public class RecordSizeLimitAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) 
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            IConfiguration configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
            
            ILoggerFactory loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            ILogger logger = loggerFactory.CreateLogger(context.Controller.GetType());

            var hasValue = context.ActionArguments.TryGetValue("json", out object value);
            
            if (hasValue)
            {
                try
                {
                    User user = (User)context.HttpContext.Items["User"];

                    JObject record = JObject.FromObject(value);
                    string json = record.ToString(Formatting.None);
                    long bytes = json.Length * sizeof(char);
                    string suffix = GetSizeSuffix(bytes);

                    long.TryParse(configuration["JSONMaxBytes"], out long jsonMaxBytes);

                    double threshold = jsonMaxBytes * 0.001;

                    if (bytes > jsonMaxBytes)
                    {
                        logger.LogWarning($"There was an attempt to stash {suffix} of json by user id: {user.UserGuid}.");

                        context.Result = new JsonResult(new { message = $"The json you wish to stash is larger than {threshold} kbs. Current size: {suffix}" }) { StatusCode = StatusCodes.Status413PayloadTooLarge };
                    }
                }
                catch
                {
                    context.Result = new JsonResult(new { message = $"There was an issue with your request. Please, refer to api documentation on proper json format for stashing." }) { StatusCode = StatusCodes.Status400BadRequest };
                }
            }
        }

        /// <summary>
        /// Convert bytes to it's relative size.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimalPlaces"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// Solution Found Here: https://stackoverflow.com/a/14488941
        private static string GetSizeSuffix(long value, int decimalPlaces = 1)
        {
            string[] sizes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + GetSizeSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            int mag = (int)Math.Log(value, 1024);

            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", adjustedSize, sizes[mag]);
        }
    }
}
