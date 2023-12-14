using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BadNews.Elevation
{
    public class ElevationMiddleware
    {
        private readonly RequestDelegate next;
    
        public ElevationMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
    
        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            if (path == "/elevation")
            {
                var upQuery = context.Request.Query["up"];
                if (upQuery.Count != 0)
                {
                    context.Response.Cookies.Append(ElevationConstants.CookieName, ElevationConstants.CookieValue,
                        new CookieOptions
                        {
                            HttpOnly = true
                        });
                }
                else
                {
                    context.Response.Cookies.Delete(ElevationConstants.CookieName);
                }
                
                await next(context);
                context.Response.Redirect("/");
            }
            else
            {
                await next(context);
            }
        }
    }
}
