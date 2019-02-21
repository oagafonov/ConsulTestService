using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Consul;
using Microsoft.AspNetCore.Mvc;

namespace ConsulTestService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Route("inc")]
        public ActionResult<int> Increment()
        {
            var client = new ConsulClient();
            var l = client.KV.Get("ois-test-value");
            int value = 0;
            if (l?.Result?.Response?.Value != null && l.Result.Response.Value.Any())
            {
                value = Int32.Parse(Encoding.ASCII.GetString(l.Result.Response.Value));
                value++;
            }

            var pair = new KVPair("ois-test-value")
            {
                Value = Encoding.ASCII.GetBytes(value.ToString())
            };
            client.KV.Put(pair);

            return value;
        }
    }
}
