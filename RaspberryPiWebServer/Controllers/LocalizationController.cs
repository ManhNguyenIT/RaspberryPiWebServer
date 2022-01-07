using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RaspberryPiWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalizationController : ControllerBase
    {
        [HttpGet("CldrData")]
        public IActionResult CldrData()
        {
            return new CldrDataScriptBuilder()
                .SetCldrPath("~/wwwroot/cldr-data")
                .SetInitialLocale("en")
                .UseLocales(new[] { "en", "vi" })
                .Build();
        }
    }
}
