using Acc.Api.Authorize;
using Acc.Api.Helper;
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

namespace Acc.Api.Controllers.WorkshopUser
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkshopUserController : ControllerBase
    {
        private FmDriverService DriverService;
        //
        public WorkshopUserController(IConfiguration configuration, IHostingEnvironment environment)
        {
            DriverService = new FmDriverService(configuration);
        }
        [HttpPost]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult SaveJson(MMWorkshopUser Model)
        {
            var output = new Output();
            try
            {
                output = DriverService.SaveWoUser(Model);
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
        public IActionResult UpdateJson(MMWorkshopUser Model)
        {
            var output = new Output();
            try
            {
                output = DriverService.UpdateWoUser(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

    }
}
