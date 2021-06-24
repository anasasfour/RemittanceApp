using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemittanceApp.Error;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RemittanceApp.Middlewares
{
    public class ExceptionMiddleware
    {
        #region Properties
        public readonly RequestDelegate Next;
        public readonly ILogger Logger;
        private readonly IWebHostEnvironment env;
        #endregion

        #region Constr
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            this.Next = next;
            this.Logger = logger;
            this.env = env;
        }
        #endregion

        #region Invoke Method
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await Next(context);
            }
            catch (Exception ex)
            {
                string message;
                ApiError response;
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                var exceptionType = ex.GetType();

                if (exceptionType == typeof(UnauthorizedAccessException))
                {
                    statusCode = HttpStatusCode.Forbidden;
                    message = "You are not Authorized";
                }
                else
                {
                    message = ex.Message;
                }

                if (env.IsDevelopment())
                {
                    response = new ApiError((int)statusCode, ex.Message, ex.StackTrace);
                }
                else
                {
                    //response = new ApiError((int)statusCode, message);
                    response = new ApiError((int)statusCode, ex.Message, ex.StackTrace);
                }

                Logger.LogError(ex, JsonConvert.SerializeObject(response));
                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(response.ToString());
            }
        }
        #endregion
    }
}
