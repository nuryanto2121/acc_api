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

namespace Acc.Api.Controllers.Dashboard
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private IConfiguration config;
        private DashboardService dashboardService;
        public DashboardController(IConfiguration configuration)
        {
            dashboardService = new DashboardService(configuration);
            config = configuration;

        }
        [HttpGet("Admin")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult DashAdmin(string portfolio_id, string user_id)
        {
            var output = new Output();
            try
            {
                output = dashboardService.Admin(portfolio_id, user_id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(output);
        }

    }
}