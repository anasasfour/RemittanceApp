using RemittanceApp.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace RemittanceApp.Extensions
{
    public static class ExceptionMiddlewareExtension
    {
        #region Configure Exception
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
        #endregion
    }
}
