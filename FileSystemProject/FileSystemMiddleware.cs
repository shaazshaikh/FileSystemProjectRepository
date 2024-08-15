using System.Security.Claims;

namespace FileSystemProject
{
    public class FileSystemMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public FileSystemMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if(context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if(!string.IsNullOrEmpty(userId) )
                {
                    context.Items["UserId"] = userId;
                }
            }

            await _requestDelegate(context);
        }

    }
}
