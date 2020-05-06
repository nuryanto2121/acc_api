using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acc.Api.Helper;
using Acc.Api.Interface;
using Acc.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Acc.Api.Controllers.Sender
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailSender;
        public EmailController(IConfiguration configuration,IEmailService emailSender)
        {
            _emailSender = emailSender;
            //Tools.ConnectionString(Configuration)
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Output), 200)]
        public async Task<IActionResult> SendAsync([FromBody] EmailModel Model)
        {
            var _result = new Output();
            try
            {
                _result = await _emailSender.SendEmailAsync(Model);


            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }
            return StatusCode(StatusCodes.Status200OK, _result);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="Model"></param>
        ///// <returns></returns>
        //[HttpPost("Send")]
        //[ProducesResponseType(typeof(Output), 200)]
        //public IActionResult Send([FromBody] EmailModel Model)
        //{
        //    var _result = new Output();
        //    try
        //    {
        //        _result = _emailSender.SendEmail(Model);


        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
        //    }
        //    return StatusCode(StatusCodes.Status200OK, _result);
        //}
    }
}