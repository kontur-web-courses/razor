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
            await next(context);
            if (context.Request.Path.Value == "/elevation")
            {
                if (context.Request.Query.ContainsKey("up"))
                    context.Response.Cookies.Append(ElevationConstants.CookieName, ElevationConstants.CookieValue,
                        new CookieOptions
                        {
                            HttpOnly = true
                        });
                else
                    context.Response.Cookies.Delete(ElevationConstants.CookieName);
                
                context.Response.Redirect("/");
            }
        }
    }
}
