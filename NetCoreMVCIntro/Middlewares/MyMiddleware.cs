using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreMVCIntro.Middlewares
{


    // custom middleware
    // app.use() formatında yazılır.
    public class MyMiddleware : IMiddleware
    {
      
        public async Task InvokeAsync(HttpContext context, Microsoft.AspNetCore.Http.RequestDelegate next)
        {
            await next(context);
        }
    }
}
