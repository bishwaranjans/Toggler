using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Toggler.WebApi
{  
    /// <summary>
    /// Proxy middleware to fix CORS and impersonation issues.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next, IApplicationBuilder app, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            App = app;
            Logger = logger;
        }

        private IApplicationBuilder App { get; }
        public ILogger<ExceptionHandlingMiddleware> Logger { get; }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (HttpResourceNotFoundException ex)
            {
                if (httpContext.Response.HasStarted)
                {
                    Logger.LogWarning("The response has already started, the exception handling middleware will not be executed.");
                    throw;
                }

                await WriteErrorResponse(httpContext, ex, StatusCodes.Status404NotFound);

                return;
            }
            catch (HttpBadRequestException ex)
            {
                if (httpContext.Response.HasStarted)
                {
                    Logger.LogWarning("The response has already started, the exception handling middleware will not be executed.");
                    throw;
                }

                await WriteErrorResponse(httpContext, ex, StatusCodes.Status400BadRequest);
                return;
            }
            catch (Exception ex)
            {
                if (httpContext.Response.HasStarted)
                {
                    Logger.LogWarning("The response has already started, the exception handling middleware will not be executed.");
                    throw;
                }

                await WriteErrorResponse(httpContext, ex, StatusCodes.Status500InternalServerError);

                return;
            }
        }

        private async Task WriteErrorResponse(HttpContext httpContext, Exception ex, int status)
        {
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = status;
            httpContext.Response.ContentType = @"application/json";

            var error = new ErrorInfo
            {
                Id = Guid.NewGuid().ToString(),
                Message = ex.Message,
            };

            var exceptionJson = JsonConvert.SerializeObject(error);
            await httpContext.Response.WriteAsync(exceptionJson);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlingMiddlewareeExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>(builder);
        }
    }
}
