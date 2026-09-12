using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace PL.Attribute
{
    public class IdempotencyAttribute : ActionFilterAttribute
    {
        private const string IdempotencyKeyHeader = "Idempotency-Key";
        private const string InProgressStatus = "InProgress";
        private class cachedResponse
        {
            public int StatusCode { get; set; }
            public Object? Value { get; set; }
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(!context.HttpContext.Request.Headers.TryGetValue(IdempotencyKeyHeader, out var idempotencyKey) || string.IsNullOrWhiteSpace(idempotencyKey))
            {
                context.Result = new BadRequestObjectResult($"Missing or empty {IdempotencyKeyHeader} header.");
                return;
            }
            var cacheKey = $"Idempotency_{idempotencyKey}";

            var Cache = context.HttpContext.RequestServices.GetService<IDistributedCache>();
            var CahchedResponseJson = await Cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(CahchedResponseJson))
            {
                if (CahchedResponseJson == InProgressStatus)
                {
                    context.Result = new StatusCodeResult(409); // Conflict
                    return;
                }
                var cachedResponce = JsonSerializer.Deserialize<cachedResponse>(CahchedResponseJson);
                if (cachedResponce != null)
                {
                    context.Result = new ObjectResult(cachedResponce.Value)
                    {
                        StatusCode = cachedResponce.StatusCode
                    };
                    return;
                } 
            }else
            {
                await Cache.SetStringAsync(cacheKey, InProgressStatus, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                });
                var executedContext = await next();
                if(executedContext.Result is  ObjectResult objectResult)
                {
                    if (objectResult.StatusCode >= 200 && objectResult.StatusCode < 300)
                    {
                        var responseToCache = new cachedResponse
                        {
                            StatusCode = objectResult.StatusCode ?? 200,
                            Value = objectResult.Value
                        };
                        var responseJson = JsonSerializer.Serialize(responseToCache);
                        await Cache.SetStringAsync(cacheKey, responseJson, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                        });
                    }
                    else
                    {
                        await Cache.RemoveAsync(cacheKey);
                    }

                }
            }


            
        }
    }
}
