using System.IO.Pipelines;
using System.Text;
using System.Text.Json;

namespace BodyReaderBugDemo.Extensions
{
    public static class HttpRequestExtensions
    {
        public static async Task<string?> ReadBody(this HttpRequest httpRequest, Encoding? encoding = default)
        {
            encoding ??= Encoding.UTF8;

            if (!httpRequest.Body.CanSeek)
            {
                // We only do this if the stream isn't *already* seekable, as `EnableBuffering`
                // will create a new stream instance each time it's called
                httpRequest.EnableBuffering();
            }

            httpRequest.Body.Seek(0, SeekOrigin.Begin);

            var bodyReader = httpRequest.BodyReader;

            var context = httpRequest.HttpContext;

            ReadResult readResult = default;

            try
            {
                // read all the body without consuming it
                do
                {
                    // read sequence of bytes from the pipe reader
                    readResult = await bodyReader.ReadAsync(context.RequestAborted);

                    // Tell the PipeReader how much of the buffer we have consumed
                    bodyReader.AdvanceTo(readResult.Buffer.Start, readResult.Buffer.End);

                } while (!readResult.IsCanceled && !readResult.IsCompleted);

                // now all the body payload has been read into buffer
                var buffer = readResult.Buffer;

                if (buffer.IsEmpty)
                    return default;

                // process the buffer
                return encoding.GetString(buffer);
            }
            finally
            {
                // Finally, reset the examined position here
                bodyReader.AdvanceTo(readResult.Buffer.Start, readResult.Buffer.Start);
            }
        }

        public static T? ReadBody<T>(this HttpRequest httpRequest)
        {
            if (!httpRequest.Body.CanSeek)
            {
                // We only do this if the stream isn't *already* seekable, as `EnableBuffering`
                // will create a new stream instance each time it's called
                httpRequest.EnableBuffering();
            }

            httpRequest.Body.Seek(0, SeekOrigin.Begin);

            var bodyReader = httpRequest.BodyReader;

            ReadResult readResult = default;

            try
            {
                // read all the body without consuming it
                do
                {
                    // try to read sequence of bytes from the pipe reader
                    if (!bodyReader.TryRead(out readResult))
                        break;

                    // Tell the PipeReader how much of the buffer we have consumed
                    bodyReader.AdvanceTo(readResult.Buffer.Start, readResult.Buffer.End);

                } while (!readResult.IsCanceled && !readResult.IsCompleted);

                // now all the body payload has been read into buffer
                var buffer = readResult.Buffer;

                if (buffer.IsEmpty)
                    return default;

                // process the buffer
                var reader = new Utf8JsonReader(buffer);

                return JsonSerializer.Deserialize<T>(ref reader, JsonExtensions.JsonOptions);
            }
            finally
            {
                // Finally, reset the examined position here
                bodyReader.AdvanceTo(readResult.Buffer.Start, readResult.Buffer.Start);
            }
        }
    }
}
