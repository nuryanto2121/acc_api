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
        /// <param name="captcha"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetTracking([Required] string order_no, [Required] string captcha)
        {
            var _result = new Output();
            try
            {
                _result = trackingService.GetDataTracking(order_no, captcha);
                if (_result.Error)
                {

                    var except = Tools.ErrStatusCode(_result.Message);
                    _result.Status = 500;
                    _result.Message = except.Message;
                    return StatusCode(except.StatusCode, _result);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, Tools.Error(ex));
            }
            return this.Ok(_result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("genCaptcha")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult GetCaptcha()
        {
            var _result = new Output();
            try
            {
                _result = trackingService.GenCaptcha();

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, Tools.Error(ex));
            }
            return this.Ok(_result);
        }
    }
}
