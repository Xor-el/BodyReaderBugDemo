using BodyReaderBugDemo.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BodyReaderBugDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BodyReaderBugDemoController : ControllerBase
    {

        private readonly ILogger<BodyReaderBugDemoController> _logger;

        public BodyReaderBugDemoController(ILogger<BodyReaderBugDemoController> logger)
        {
            _logger = logger;
        }

        [HttpPost("working")]
        [ServiceFilter(typeof(ExtractRequestBodyWorking))]
        public IActionResult Working()
        {
            return Ok();
        }

        [HttpPost("buggy")]
        [ServiceFilter(typeof(ExtractRequestBodyBuggy))]
        public IActionResult Buggy()
        {
            return Ok();
        }
    }
}
