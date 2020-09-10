using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Acc.Api.Helper;
using Acc.Api.Interface;
using Acc.Api.Models;
using Acc.Api.Services;
using System.ComponentModel.DataAnnotations;

namespace Acc.Api.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class AuthController : ControllerBase, IAuthAPI
    {
        private IAuthService AuthService;
        private readonly IEmailService _emailSender;
        public AuthController(IConfiguration configuration, IEmailService emailSender)
        {
            AuthService = new AuthServices(configuration, emailSender);
            _emailSender = emailSender;
        }

        /// <summary>
        /// change password
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Auth/ChangePassword")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult ChangePassword([FromBody] ChangePassword Model)
        {
            var _result = new Output();
            try
            {
                _result =  AuthService.ChangePassword(Model);

            }
            catch (Exception ex)
            {
                //_result = Tools.Error(ex);
                return StatusCode(StatusCodes.Status409Conflict, Tools.Error(ex));
            }
            return this.Ok(_result);
        }

        /// <summary>
        /// kirim OTP utk mendapatkan user id kemudian change password
        /// </summary>
        /// <param name="OTP"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("Auth/Validate")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Validate([Required]string OTP)
        {
            var _result = new Output();
            try
            {
                _result = AuthService.Validate(OTP);
            }
            catch (Exception ex)
            {
                //_result = Tools.Error(ex);
                return StatusCode(StatusCodes.Status409Conflict, Tools.Error(ex));
            }
            return this.Ok(_result);
        }

        /// <summary>
        /// ini adalah login
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Auth/Login")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Login([FromBody] AuthLogin Model)
        {
            var _result = new Output();
            try
            {
                _result = this.AuthService.Login(Model);
                if (_result.Error)
                {
                    
                    var except = Tools.ErrStatusCode(_result.Message);
                    _result.Status = except.StatusCode;
                    _result.Message = except.Message;
                    return StatusCode(except.StatusCode, _result);
                }
            }
            catch (Exception ex)
            {
                var except = Tools.ErrStatusCode(ex);
                return StatusCode(except.StatusCode, Tools.Error(except.Message, except.StatusCode));
            }
            return StatusCode(StatusCodes.Status200OK, _result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Auth/Logout")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Logout(AuthLogin Model)
        {
            var _result = new Output();
            try
            {
                _result = this.AuthService.Logout(Model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, Tools.Error(ex));
            }
            return StatusCode(StatusCodes.Status200OK, _result);
        }


        [AllowAnonymous]
        [HttpPost("Auth/Register")]
        [ProducesResponseType(typeof(Output), 200)]
        public IActionResult Register([FromBody] ChangePassword Model)
        {
            var _result = new Output();
            try
            {

            }
            catch (Exception ex)
            {
                var except = Tools.ErrStatusCode(ex);
                return StatusCode(except.StatusCode, Tools.Error(except.Message, except.StatusCode));
            }
            return StatusCode(StatusCodes.Status200OK, _result);
        }

        /// <summary>
        /// Forgot Password masukan email, setelah dapat OTP jalankan Validate
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Auth/ForgotPassword")]
        [ProducesResponseType(typeof(Output), 200)]
        public async Task<IActionResult> ResetPassword([FromBody] ForgotPassword Model)
        {
            var _result = new Output();
            try
            {
                _result = await AuthService.ForgotPassword(Model);
                //IEmailService _emailSender
                // _emailSender
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
                //_result = Tools.Error(ex);
            }
            return this.Ok(_result);
        }
    }
}