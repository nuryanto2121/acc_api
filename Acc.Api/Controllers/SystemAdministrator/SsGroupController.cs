using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acc.Api.Authorize;
using Acc.Api.Helper;
using Acc.Api.Models;
using Acc.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Acc.Api.Controllers.SystemAdministrator
{
    [Route("api/[controller]")]
    [ApiController]
    public class SsGroupController : ControllerBase
    {
        private IConfiguration config;
        private SsGroupService SsGroupService;
        public SsGroupController(IConfiguration configuration)
        {
            SsGroupService = new SsGroupService(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio_id"></param>
        /// <param name="group_id"></param>
        /// <returns></returns>
        [HttpGet("Json")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetMenuJson(string portfolio_id, string group_id)
        {
            var output = new Output();
            try
            {
                output = SsGroupService.GetMenuJson(portfolio_id, group_id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult SaveJson(VmSsGroup Model)
        {
            var output = new Output();
            try
            {
                output = SsGroupService.Save(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPut]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult UpdateJson(VmSsGroup Model)
        {
            var output = new Output();
            try
            {
                output = SsGroupService.Update(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }



    }
}