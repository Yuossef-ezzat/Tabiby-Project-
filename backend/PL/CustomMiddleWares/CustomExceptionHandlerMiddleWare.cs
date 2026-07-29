using DAL.Exceptions;
using Shared.ErrorModels;

namespace TalabatDemo.CustomMiddleWares
{
    public class CustomExceptionHandlerMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionHandlerMiddleWare> _logger;

        public CustomExceptionHandlerMiddleWare(RequestDelegate Next , ILogger<CustomExceptionHandlerMiddleWare> logger)
        {
            _next = Next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext Context)
        {
            
            try
            {
                await _next.Invoke(Context);
                await HandleNotFoundEndPointAsync(Context);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandlerExceptionAsync(Context, ex);
            }

        }

        private static async Task HandlerExceptionAsync(HttpContext Context, Exception ex)
        {
            var response = new ErrorToReturn 
            {
                ErrorMsg = ex.Message
            };
            Context.Response.StatusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                BadRequestException bad => GetBadRequestErrors(bad, response),
                _ => StatusCodes.Status500InternalServerError
            };

            response.StutsCode = Context.Response.StatusCode;


            await Context.Response.WriteAsJsonAsync(response);
        }

        private static int GetBadRequestErrors(BadRequestException badRequest , ErrorToReturn responce )
        {
            responce.errors = badRequest.Errors;

            return StatusCodes.Status400BadRequest;
        }
        private static async Task HandleNotFoundEndPointAsync(HttpContext Context)
        {
            if (Context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                var response = new ErrorToReturn
                {
                    StutsCode = StatusCodes.Status404NotFound,
                    ErrorMsg = $"The Requested Url '{Context.Request.Path}' is not found"
                };


                await Context.Response.WriteAsJsonAsync(response);

            }
        }
    }
}
