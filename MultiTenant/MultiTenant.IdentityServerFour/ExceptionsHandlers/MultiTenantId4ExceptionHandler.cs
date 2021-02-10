using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenant.IdentityServerFour.ExceptionsHandlers
{
    public class MultiTenantId4ExceptionHandler
    {
        private readonly ILogger<MultiTenantId4ExceptionHandler> _logger;
        private readonly RequestDelegate _next;

        public MultiTenantId4ExceptionHandler(ILogger<MultiTenantId4ExceptionHandler> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
              await  _next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occured");
               await HandleExceptionAsync(context, e);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.Redirect("/error/index");
        }
    }
}
