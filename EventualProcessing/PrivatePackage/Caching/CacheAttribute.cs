using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PrivatePackage.Abstractions;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivatePackage.Caching
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
            if (context.HttpContext.RequestServices.GetService(typeof(IRedisSettings)) is RedisSettings)
            {
                var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
                var cacheKey = context.HttpContext.Request.GenerateCacheKey();
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

    public static class CacheAttributeExtensions
    {
        public static string GenerateCacheKey2(this HttpRequest request)
        {
            return request.Path;
        }
        public static string GenerateCacheKey(this HttpRequest request)
        {
            var cacheKeyBuilder = new StringBuilder();
            cacheKeyBuilder = RemoveUnorderedQueryString(cacheKeyBuilder, request.Path);
            cacheKeyBuilder = BuildOrderedQueryString(cacheKeyBuilder, request.Query);

            return cacheKeyBuilder.ToString();
        }

        private static StringBuilder RemoveUnorderedQueryString(StringBuilder builder, PathString path)
        {
            builder.Append(path);
            var queryStartIndex = path.Value.IndexOf('?');
            if (queryStartIndex > -1) builder.Remove(path.Value.IndexOf('?'), path.Value.Length - queryStartIndex);

            return builder;
        }

        private static StringBuilder BuildOrderedQueryString(StringBuilder queryStringBuilder, IQueryCollection queryCollection)
        {
            if (queryCollection.Any())
            {
                var isFirstPair = true;

                queryStringBuilder = queryCollection.OrderBy(q => q.Key)
                    .Aggregate(queryStringBuilder, (builder, currentPair) =>
                    {
                        builder.Append(isFirstPair ? "?" : "&")
                            .Append(currentPair.Key)
                            .Append('=')
                            .Append(currentPair.Value);
                        isFirstPair = false;
                        return builder;
                    });
            }

            return queryStringBuilder;
        }
    }
}
