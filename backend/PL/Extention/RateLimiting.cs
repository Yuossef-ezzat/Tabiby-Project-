using System.Net;
using System.Threading.RateLimiting;

namespace PL.Extention
{
    public static class RateLimiting
    {
        public static IServiceCollection AddCustomRateLimiting(
        this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddPolicy("Auth", context =>
                    RateLimitPartition.GetTokenBucketLimiter($"ip:{context.Connection.RemoteIpAddress}" ,
                    _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 10,
                        QueueLimit = 0,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        TokensPerPeriod = 10,
                        AutoReplenishment = true
                    }));


                options.AddPolicy("CreateOrder", context =>
                    RateLimitPartition.GetTokenBucketLimiter(GetPartitionKey(context),
                    _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 10,
                        QueueLimit = 0,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        TokensPerPeriod = 5,
                        AutoReplenishment = true
                    }));

                options.AddPolicy("Payment", context =>
                    RateLimitPartition.GetTokenBucketLimiter(
                        GetPartitionKey(context),

                        _ => new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = 5,
                            TokensPerPeriod = 5,
                            ReplenishmentPeriod =TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }));
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.ContentType =
                        "application/json";

                    await context.HttpContext.Response.WriteAsJsonAsync(
                        new
                        {
                            message = "Too many requests. Please try again later."
                        },
                        cancellationToken);
                };
            });

            return services;
        }
        private static string GetPartitionKey(
            HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                return $"user:{context.User.Identity.Name}";
            }

            return $"ip:{context.Connection.RemoteIpAddress}";
        }
    }
}
