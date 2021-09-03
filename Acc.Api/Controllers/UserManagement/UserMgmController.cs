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

namespace Acc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserMgmController : ControllerBase
    {
        private UserMgmService UserMgmService;
        public UserMgmController(IConfiguration configuration, IHostingEnvironment environment)
        {
            UserMgmService = new UserMgmService(configuration);
        }

        [HttpPost]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult SaveJson(UserManagement Model)
        {
            var output = new Output();
            try
            {
                output = UserMgmService.Save(Model);
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
        public IActionResult Update(UserManagement Model)
        {
            var output = new Output();
            try
            {
                output = UserMgmService.Update(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }
    }
}
