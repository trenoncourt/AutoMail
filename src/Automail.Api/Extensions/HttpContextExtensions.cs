using Microsoft.AspNetCore.Http;

namespace Automail.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static bool HasAuthorizedContentType(this HttpContext context)
        {
            return context.Request.HasFormContentType && context.Request.Form.ContainsKey("data") || 
                   context.Request.ContentType.Contains("application/json");
        }
    }
}