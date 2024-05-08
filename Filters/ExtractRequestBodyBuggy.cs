using BodyReaderBugDemo.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BodyReaderBugDemo.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExtractRequestBodyBuggy : BaseRequestFilter, IAsyncResourceFilter
    {
        public ExtractRequestBodyBuggy()
        {
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            var data = ReadBodyFromRequest<BodyReaderBugRequest>(request);

            if (data is null)
            {
                context.Result = new BadRequestResult();
                return;
            }

            await next();
        }
    }
}
