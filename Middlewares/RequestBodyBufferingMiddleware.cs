namespace BodyReaderBugDemo.Middlewares
{
    public class RequestBodyBufferingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            context.Request.EnableBuffering();

            await next(context);
        }
    }
}
