using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreMVCIntro.Middlewares
{
    //public class MyMiddleware
    //{
    //    private readonly RequestDelegate _next;

    //    public MyMiddleware(RequestDelegate next)
    //    {
    //        _next = next;
    //    }

    //    public async Task InvokeAsync(HttpContext httpContext)
    //    {

    //        await _next(httpContext);
    //    }
 
    //}

    public class MyMiddleware2 : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, Microsoft.AspNetCore.Http.RequestDelegate next)
        {
                // await context.Response.WriteAsync("Hello World!");
            await next.Invoke(context);
        }
    }
}
