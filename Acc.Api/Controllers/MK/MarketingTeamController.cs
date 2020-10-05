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

namespace Acc.Api.Controllers.MK
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketingTeamController : ControllerBase
    {
        private MarketingTeamService marketingService;
        public MarketingTeamController(IConfiguration configuration)
        {
            marketingService = new MarketingTeamService(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio_id"></param>
        /// <param name="user_id"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Getlist(string portfolio_id, string user_id)
        {
            var output = new Output();
            try
            {
                output = marketingService.GetLIst(user_id, portfolio_id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

        [HttpPost]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Save(MarketingTeamParam Model)
        {
            var output = new Output();
            try
            {
                output = marketingService.Save(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }
    }
}