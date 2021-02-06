using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]"), ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public OperationsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpOptions("reloadconfig")]
        public IActionResult ReloadConfig()
        {
            try
            {
                IConfigurationRoot root = (IConfigurationRoot) this.configuration;

                root.Reload();
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}