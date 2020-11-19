using Acc.Api.Helper;
using Acc.Api.Models;
using Acc.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Controllers.Tracking
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        private TrackingService trackingService;
        public TrackingController(IConfiguration configuration)
        {
            trackingService = new TrackingService(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order_no"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult ChangePassword([Required] string order_no)
        {
            var _result = new Output();
            try
            {
                _result = trackingService.GetDataTracking(order_no);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, Tools.Error(ex));
            }
            return this.Ok(_result);
        }

    }
}
