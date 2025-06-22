using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Test_WebAPI_8.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestConfigsController : ControllerBase
    {
        private readonly IOptionsSnapshot<Ftp> _ftpOpt;
        private readonly IOptionsSnapshot<Cors> _corsOpt;

        public TestConfigsController(IOptionsSnapshot<Ftp> ftpOpt, IOptionsSnapshot<Cors> corsOpt)
        {
            _ftpOpt = ftpOpt;
            _corsOpt = corsOpt;
        }

        [HttpGet]
        public string GetConfigs()
        {
            return _ftpOpt.Value + "" + _corsOpt.Value;
        }
    }
}
