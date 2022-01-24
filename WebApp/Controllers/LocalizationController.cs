using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalizationController : ControllerBase
    {
        private readonly Settings _settings;

        public LocalizationController(IOptions<Settings> settings)
        {
            _settings=settings?.Value;
        }
        [HttpGet("CldrData")]
        public IActionResult CldrData()
        {
            return new CldrDataScriptBuilder()
                .SetCldrPath("~/wwwroot/cldr-data")
                .SetInitialLocale("en")
                .UseLocales(new[] { "en", "vi" })
                .Build();
        }

        [HttpPost("check-password")]
        public IActionResult CheckPassword([FromForm] string password)
        {
            return Ok(password==_settings?.Password);
        }
    }
}
