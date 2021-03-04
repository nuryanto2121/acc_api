using Acc.Api.Authorize;
using Acc.Api.Helper;
using Acc.Api.Interface;
using Acc.Api.Models;
using Acc.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Controllers.Driver
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        //private IConfiguration config;
        private FmDriverService DriverService;

        public DriverController(IConfiguration configuration, IHostingEnvironment environment)
        {
            DriverService = new FmDriverService(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult SaveJson(VmFMDriver Model)
        {
            var output = new Output();
            try
            {
                output = DriverService.Save(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

        [HttpPut]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult UpdateJson(VmFMDriver Model)
        {
            var output = new Output();
            try
            {
                output = DriverService.Update(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }
    }
}
