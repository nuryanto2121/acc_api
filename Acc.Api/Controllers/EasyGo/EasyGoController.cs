using Acc.Api.Authorize;
using Acc.Api.Helper;
using Acc.Api.Models;
using Acc.Api.Services;
using Microsoft.AspNetCore.Authorization;
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
    public class EasyGoController : ControllerBase
    {
        private EasyGoService easyGoService;
        public EasyGoController(IConfiguration configuration, IHostingEnvironment environment)
        {
            easyGoService = new EasyGoService(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [APIAuthorizeAttribute]
        [HttpPost("DoV1")]
        [ProducesResponseType(typeof(Output), 200)]
        public async Task<IActionResult> DoV1([FromBody] EasyGoAdd Model)
        {
            var _result = new Output();
            try
            {
                _result = await easyGoService.DOV1(Model);
                if (_result.Error)
                {
                    return StatusCode(501, _result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }
            return this.Ok(_result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Model"></param>
        /// <returns></returns>
        [APIAuthorizeAttribute]
        [HttpPut("CloseDoV1/{id}")]
        [ProducesResponseType(typeof(Output), 200)]
        public async Task<IActionResult> CloseDoV1(int id,[FromBody] EasyGoCloasParam Model)
        {
            var _result = new Output();
            try
            {
                _result = await easyGoService.CloaseDoV1(id,Model);
                if (_result.Error)
                {
                    return StatusCode(501, _result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }
            return this.Ok(_result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [APIAuthorizeAttribute]
        [HttpPost("LastPosition")]
        [ProducesResponseType(typeof(Output), 200)]
        public async Task<IActionResult> LastPosition([FromBody] EasyGoLastPositionParam Model)
        {
            var _result = new Output();
            try
            {
                _result = await easyGoService.LastPosition(Model);
                if (_result.Error)
                {
                    return StatusCode(501, _result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }
            return this.Ok(_result);
        }
    }
}
