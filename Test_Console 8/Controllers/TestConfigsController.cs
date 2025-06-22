using Microsoft.Extensions.Options;

namespace Test_Console_8.Controllers
{
    public class TestConfigsController
    {
        private readonly IOptionsSnapshot<Ftp> _ftpOpt;
        private readonly IOptionsSnapshot<Cors> _corsOpt;

        public TestConfigsController(IOptionsSnapshot<Ftp> ftpOpt, IOptionsSnapshot<Cors> corsOpt)
        {
            _ftpOpt = ftpOpt;
            _corsOpt = corsOpt;
        }

        public void GetConfigs()
        {
            var result = _ftpOpt.Value + "" + _corsOpt.Value;
            Console.WriteLine(result);
        }
    }
}
