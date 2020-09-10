using Microsoft.AspNetCore.Mvc;
using Acc.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Interface
{
    public interface IAuthAPI
    {
        IActionResult Login([FromBody]AuthLogin Model);
        IActionResult Logout([FromBody]AuthLogin Model);
        IActionResult ChangePassword([FromBody]ChangePassword Model);
        IActionResult Register([FromBody]ChangePassword Model);
        //IActionResult ResetPassword([FromBody]ChangePassword Model);
        Task<IActionResult> ResetPassword([FromBody] ForgotPassword Model);
    }

    public interface IAuthService
    {
        Output Login(AuthLogin Model);
        Output Logout(AuthLogin Model);
        Output ChangePassword(ChangePassword Model);
        Output Validate(string OTP);
        //Output ForgotPassword(ForgotPassword Model);
        Task<Output> ForgotPassword(ForgotPassword Model);
    }

}
