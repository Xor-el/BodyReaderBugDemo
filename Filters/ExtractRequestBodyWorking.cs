using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BodyReaderBugDemo.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExtractRequestBodyWorking : BaseRequestFilter, IAsyncResourceFilter
    {
        public ExtractRequestBodyWorking()
        {
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            var data = await ReadBodyFromRequest(request);

            if (data is null)
            {
                context.Result = new BadRequestResult();
                return;
            }

            await next();
        }
    }
}
