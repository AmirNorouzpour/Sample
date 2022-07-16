using System.Globalization;
using System.Net;
using API.Models;
using Common;
using Common.Exception;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace API.Middlewares
{
    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }

    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            var message = string.Empty;
            var httpStatusCode = HttpStatusCode.InternalServerError;
            var apiStatusCode = ApiResultStatusCode.ServerError;

            try
            {
                await _next(context);
            }
            catch (AppException exception)
            {
                httpStatusCode = exception.HttpStatusCode;
                apiStatusCode = exception.ApiStatusCode;
                if (_env.IsDevelopment())
                {
                    if (exception.StackTrace != null)
                    {
                        var dic = new Dictionary<string, string>
                        {
                            ["Exception"] = exception.Message,
                            ["StackTrace"] = exception.StackTrace,
                        };
                        if (exception.InnerException != null)
                        {
                            dic.Add("InnerException.Exception", exception.InnerException.Message);
                            if (exception.InnerException.StackTrace != null)
                                dic.Add("InnerException.StackTrace", exception.InnerException.StackTrace);
                        }

                        dic.Add("AdditionalData", JsonConvert.SerializeObject(exception.AdditionalData));

                        message = JsonConvert.SerializeObject(dic);
                    }
                }
                else
                {
                    message = exception.Message;
                }

                await WriteToResponseAsync();
            }
            catch (SecurityTokenExpiredException exception)
            {
                SetUnAuthorizeResponse(exception);
                await WriteToResponseAsync();
            }
            catch (UnauthorizedAccessException exception)
            {

                SetUnAuthorizeResponse(exception);
                await WriteToResponseAsync();
            }
            catch (Exception exception)
            {
                if (_env.IsDevelopment())
                {
                    if (exception.StackTrace != null)
                    {
                        var dic = new Dictionary<string, string>
                        {
                            ["Exception"] = exception.Message,
                            ["StackTrace"] = exception.StackTrace,
                        };
                        message = JsonConvert.SerializeObject(dic);
                    }
                }

                await WriteToResponseAsync();

            }

            async Task WriteToResponseAsync()
            {
                if (message != null)
                {
                    var result = new ApiResult(false, apiStatusCode, message);
                    var json = JsonConvert.SerializeObject(result);

                    context.Response.StatusCode = (int)httpStatusCode;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(json);
                }
            }

            void SetUnAuthorizeResponse(Exception exception)
            {
                httpStatusCode = HttpStatusCode.Unauthorized;
                apiStatusCode = ApiResultStatusCode.UnAuthorized;

                if (_env.IsDevelopment())
                {
                    if (exception.StackTrace == null) return;
                    var dic = new Dictionary<string, string>
                    {
                        ["Exception"] = exception.Message,
                        ["StackTrace"] = exception.StackTrace
                    };
                    if (exception is SecurityTokenExpiredException tokenException)
                        dic.Add("Expires", tokenException.Expires.ToString(CultureInfo.InvariantCulture));

                    message = JsonConvert.SerializeObject(dic);
                }
            }
        }
    }
}
