using BodyReaderBugDemo.Extensions;

namespace BodyReaderBugDemo.Filters
{
    public abstract class BaseRequestFilter : Attribute
    {
        protected static async Task<string?> ReadBodyFromRequest(HttpRequest request) => await request.ReadBody();
        protected static T? ReadBodyFromRequest<T>(HttpRequest request) => request.ReadBody<T>();
    }
}
