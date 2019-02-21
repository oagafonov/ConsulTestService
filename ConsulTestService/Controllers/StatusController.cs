using Microsoft.AspNetCore.Mvc;
using Serilog;


namespace ConsulTestService.Controllers
{
    [Route("")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private ILogger Logger => Log.ForContext<StatusController>();
        [HttpGet]
        [Route("ping")]
        public string[] Ping()
        {
            Logger.Information("service ping");
            return new[] { "OK" };
        }
    }
}
