using Library.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;

namespace Library.Caching
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CacheAttribute : ActionFilterAttribute
    {
        private readonly int timeToLiveSeconds;

        public CacheAttribute(int timeToLiveSeconds)
        {
            this.timeToLiveSeconds = timeToLiveSeconds;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.RequestServices.GetService(typeof(RedisSettings)) is RedisSettings)
            {
                var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
                var cacheKey = context.HttpContext.Request.Path;
                var cachedResponse = await cacheService.GetAsync(cacheKey);

                if (cachedResponse != null)
                {
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync(cachedResponse);
                    return;
                }

                var executedContext = await next();

                if (executedContext.Result is IResult result && result.IsSuccess())
                {
                    var cacheContent = JsonConvert.SerializeObject(result.Value, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.None,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    });
                    await cacheService.SetAsync(cacheKey, cacheContent, timeToLiveSeconds);
                }
                return;
            }

            await next();
            return;
        }
    }
}
