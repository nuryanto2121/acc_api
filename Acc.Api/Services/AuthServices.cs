﻿using Acc.Api.Helper;
using EncryptLibrary.AES256Encryption;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Acc.Api.DataAccess;
using Acc.Api.Interface;
using Acc.Api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Acc.Api.Services
{
    public class AuthServices : IAuthService
    {
        private AuthRepo authRepo;
        private SsMenuRepo menuRepo;
        private SysMenuFavoriteRepo favRepo;
        IConfiguration config;
        private string _Session_Id;
        private FunctionString fn;
        private UserFCMRepo userFCMRepo;
        private readonly IEmailService _emailSender;
        private IHostingEnvironment _environment;
        public AuthServices(IConfiguration Configuration, IEmailService EmailSender, IHostingEnvironment environment)
        {
            authRepo = new AuthRepo(Tools.ConnectionString(Configuration));
            fn = new FunctionString(Tools.ConnectionString(Configuration));
            menuRepo = new SsMenuRepo(Tools.ConnectionString(Configuration));
            favRepo = new SysMenuFavoriteRepo(Tools.ConnectionString(Configuration));
            _emailSender = EmailSender;
            config = Configuration;
            _environment = environment;
            userFCMRepo = new UserFCMRepo(Tools.ConnectionString(Configuration));
        }

        public Output ChangePassword(ChangePassword Model)
        {
            Output _result = new Output();
            Dictionary<string, object> DataUser = new Dictionary<string, object>();
            try
            {                
                if (Model.ConfirmPassword != Model.NewPassword)
                {
                    throw new Exception("New Password and Confirm must be same.");
                }
                string User_id = EncryptionLibrary.DecryptText(Model.UserId);
                SsUser dataUser = authRepo.GetDataAuthByUserId(User_id);
                if (dataUser == null)
                {
                    throw new Exception("Account Not Valid.");
                }

                Model.NewPassword = EncryptionLibrary.EncryptText(Model.NewPassword);
                authRepo.UpdatePass(dataUser.ss_user_id, Model.NewPassword);
                _result.Message = "Please Login";
                //DataUser.Add("user_id", EncryptionLibrary.EncryptText(dataUser.user_id));
                //_result.Data = DataUser;
            }
            catch (Exception ex)
            {
                throw ex;
                //_result = Tools.Error(ex);
            }
            return _result;
        }

        public Output Login(AuthLogin Model)
        {
            var _result = new Output();
            Dictionary<string, object> ObjOutput = new Dictionary<string, object>();
            Dictionary<string, object> DataUser = new Dictionary<string, object>();
            JObject Captcha = new JObject();
            var dtCaptcha = string.Empty;
            string UserID = string.Empty;
            _Session_Id = string.Empty;
            try
            {
                var UserSession = new UserSession();
                var SsUserLog = new SsUserLog();
                /*generate Captcha*/
                //1. cek count user log
                string sParamUserLog = string.Format(" ip_address = '{0}' AND is_fraud = 'true' ", Tools.GetIpAddress());
                int cnt_log = authRepo.CountUserLog(Tools.GetIpAddress());
                int cnt_captcha = config.GetValue<int>("appSetting:CountCaptcha") - 1;



                if (cnt_log >= cnt_captcha)
                {
                    dtCaptcha = EncryptionLibrary.KeyGenerator.GetUniqueKey(6);
                    Captcha.Add("Captcha", dtCaptcha);
                }
                /*end Captcha*/
                Model.PassLog = EncryptionLibrary.EncryptText(Model.PassLog);
                var dataAuth = authRepo.GetDataAuth(Model);
                if (dataAuth.Rows.Count > 0)
                {
                    UserID = dataAuth.Rows[0]["user_id"].ToString();
                }
                
                if (dataAuth.Rows.Count == 1)
                {
                    var expireDate = DateTime.Now.AddMinutes(config.GetValue<int>("appSetting:TokenExpire"));
                    _Session_Id = Token.GenerateToken(dataAuth, expireDate, Model, Tools.GetIpAddress());
                    var MenuList = this.menuList(dataAuth);
                    var FavMenu = this.favoriteMenu(dataAuth);
                    if (MenuList.Rows.Count == 0)
                    {
                        throw new Exception("Please complete the menu assignment to user");
                    }
                    if (!string.IsNullOrEmpty(Model.Captcha))
                    {
                        Model.Captcha = EncryptionLibrary.EncryptText(Model.Captcha);
                        string nParam = string.Format("{0} AND captcha = '{1}'", sParamUserLog, Model.Captcha);
                        var captcha_db = authRepo.MaxCaptchaUserLog(Tools.GetIpAddress(),Model.Captcha);

                        if (captcha_db == Model.Captcha)
                        {
                            //nParam = string.Format("{0} AND captcha != '{1}'", sParamUserLog, Model.Captcha);
                            authRepo.DeleteUserLog(Tools.GetIpAddress(), Model.Captcha);
                            //nParam = string.Format("{0} AND captcha = '{1}'", sParamUserLog, Model.Captcha);
                            authRepo.UpdateUserLogisFraud(Tools.GetIpAddress(), Model.Captcha);
                        }
                        else
                        {
                            authRepo.UpdateCaptchaLog(Tools.GetIpAddress());
                            SsUserLog.user_id = UserID;
                            SsUserLog.ip_address = Tools.GetIpAddress();
                            SsUserLog.login_date = DateTime.Now;
                            SsUserLog.token = _Session_Id;
                            SsUserLog.is_fraud = true;
                            SsUserLog.captcha = EncryptionLibrary.EncryptText(dtCaptcha);
                            SsUserLog.user_input = UserID;
                            SsUserLog.user_edit = UserID;
                            SsUserLog.time_input = DateTime.Now;
                            SsUserLog.time_edit = DateTime.Now;
                            authRepo.SaveUserLog(SsUserLog);

                            //throw new Exception("Login Failed. Wrong Captcha .^" + StatusCodes.Status401Unauthorized);
                            _result.Message = "Login Failed. Wrong Captcha .^429";
                            _result.Data = Captcha;
                            _result.Error = true;
                            _result.Status = 429;
                            return _result;
                        }
                    }


                    // insert user session for authentication

                    UserSession.user_id = UserID;
                    UserSession.expire_on = expireDate;
                    UserSession.token = _Session_Id;
                    UserSession.last_login = DateTime.Now;
                    UserSession.ip_address = Tools.GetIpAddress();
                    UserSession.user_input = UserID;
                    UserSession.user_edit = UserID;
                    UserSession.time_input = DateTime.Now;
                    UserSession.time_edit = DateTime.Now;
                    authRepo.SaveUserSession(UserSession);

                    // Insert User Log
                    authRepo.UpdateCaptchaLog(Tools.GetIpAddress());

                    SsUserLog.user_id = UserID;
                    SsUserLog.ip_address = Tools.GetIpAddress();
                    SsUserLog.login_date = DateTime.Now;
                    SsUserLog.token = _Session_Id;
                    SsUserLog.is_fraud = false;
                    SsUserLog.user_input = UserID;
                    SsUserLog.user_edit = UserID;
                    SsUserLog.time_input = DateTime.Now;
                    SsUserLog.time_edit = DateTime.Now;
                    authRepo.SaveUserLog(SsUserLog);



                    //dataAuth.pwd = "";
                    //dataAuth.user_id = Tools.EncryptString(dataAuth.user_id
                    if (!string.IsNullOrEmpty(Model.TokenFCM))
                    {
                        userFCMRepo.SaveUserFCM(Convert.ToInt32(dataAuth.Rows[0]["portfolio_id"].ToString()), UserID, Model.TokenFCM, _Session_Id);
                    }
                    
                    //var dataSpec = authRepo.GetDataMkSpec(Convert.ToInt32(dataAuth.Rows[0]["portfolio_id"].ToString()));
                   
                    dataAuth = fn.DataClearEncrypt(dataAuth);

                    DataUser.Add("user_id", dataAuth.Rows[0]["user_id"].ToString());
                    DataUser.Add("user_name", dataAuth.Rows[0]["user_name"].ToString());
                    DataUser.Add("path_file", dataAuth.Rows[0]["path_file"].ToString());
                    DataUser.Add("group_id", dataAuth.Rows[0]["ss_group_id"].ToString());
                    DataUser.Add("dashboard_url", dataAuth.Rows[0]["dashboard_url"].ToString());
                    DataUser.Add("subportfolio_id", dataAuth.Rows[0]["subportfolio_id"].ToString());
                    DataUser.Add("subportfolio_short_name", dataAuth.Rows[0]["subportfolio_short_name"].ToString());
                    DataUser.Add("subportfolio_name", dataAuth.Rows[0]["subportfolio_name"].ToString());
                    DataUser.Add("portfolio_id", dataAuth.Rows[0]["portfolio_id"].ToString());
                    DataUser.Add("portfolio_short_name", dataAuth.Rows[0]["portfolio_short_name"].ToString());
                    DataUser.Add("portfolio_name", dataAuth.Rows[0]["portfolio_name"].ToString());
                    DataUser.Add("phone_country_code", dataAuth.Rows[0]["phone_country_code"].ToString()); //disable utk hoonian
                    DataUser.Add("gps_token", dataAuth.Rows[0]["gps_token"].ToString());
                    DataUser.Add("gps_map_token", dataAuth.Rows[0]["gps_map_token"].ToString());

                    ObjOutput.Add("data_user", DataUser);
                    ObjOutput.Add("token", _Session_Id);
                    ObjOutput.Add("idle", config.GetValue<int>("appSetting:IdleWeb"));
                    ObjOutput.Add("menu", MenuList);
                    ObjOutput.Add("shorcut_menu", FavMenu);

                    if (Captcha["Captcha"] != null)
                    {
                        Captcha["Captcha"] = "";
                    }
                    else
                    {
                        ObjOutput.Add("Captcha", "");
                    }
                    _result.Data = ObjOutput;// dataAuth;
                    
                }
                else
                {
                    if (!string.IsNullOrEmpty(Model.Captcha))
                    {
                        Model.Captcha = EncryptionLibrary.EncryptText(Model.Captcha);
                        sParamUserLog = string.Format("{0} AND captcha = '{1}'", sParamUserLog, Model.Captcha);
                        var captcha_db = authRepo.MaxCaptchaUserLog(Tools.GetIpAddress(), Model.Captcha);

                        if (captcha_db != Model.Captcha)
                        {
                            SsUserLog.user_id = UserID;
                            SsUserLog.ip_address = Tools.GetIpAddress();
                            SsUserLog.login_date = DateTime.Now;
                            SsUserLog.token = _Session_Id;
                            SsUserLog.is_fraud = true;
                            SsUserLog.captcha = EncryptionLibrary.EncryptText(dtCaptcha);
                            SsUserLog.user_input = UserID;
                            SsUserLog.user_edit = UserID;
                            SsUserLog.time_input = DateTime.Now;
                            SsUserLog.time_edit = DateTime.Now;
                            authRepo.SaveUserLog(SsUserLog);

                            //throw new Exception("Login Failed. Wrong Captcha .^" + StatusCodes.Status401Unauthorized);
                            _result.Message = "The user name or password is incorrect.^" + StatusCodes.Status401Unauthorized; //"Login Failed. Wrong Captcha .^429";
                            _result.Error = true;
                            _result.Data = Captcha;
                            return _result;
                        }
                    }
                    authRepo.UpdateCaptchaLog(Tools.GetIpAddress());
                    SsUserLog.user_id = UserID;
                    SsUserLog.ip_address = Tools.GetIpAddress();
                    SsUserLog.login_date = DateTime.Now;
                    SsUserLog.token = _Session_Id;
                    SsUserLog.is_fraud = true;
                    SsUserLog.captcha = !string.IsNullOrEmpty(dtCaptcha) ? EncryptionLibrary.EncryptText(dtCaptcha) : "";
                    SsUserLog.user_input = UserID;
                    SsUserLog.user_edit = UserID;
                    SsUserLog.time_input = DateTime.Now;
                    SsUserLog.time_edit = DateTime.Now;
                    authRepo.SaveUserLog(SsUserLog);
                    if (dataAuth.Rows.Count > 1)
                    {
                        _result.Message = "Duplicate Account Please Contact Your Administrator.^" + StatusCodes.Status401Unauthorized;
                    }
                    else
                    {
                        _result.Message = "The user name or password is incorrect.^" + StatusCodes.Status401Unauthorized;
                    }
                    
                    _result.Error = true;
                    _result.Data = Captcha;
                    return _result;
                    //throw new Exception("The user name or password is incorrect.^" + StatusCodes.Status401Unauthorized);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        private bool failedCaptcha(AuthLogin Model)
        {
            bool _result = false;
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public DataTable menuList(DataTable dataUser)
        {
            int GroupId = Convert.ToInt32(dataUser.Rows[0]["ss_group_id"]);
            int PortfolioId = Convert.ToInt32(dataUser.Rows[0]["portfolio_id"]);
            string UserID = dataUser.Rows[0]["user_id"].ToString();
            return menuRepo.getMenuGroup(PortfolioId, GroupId,UserID);
        }

        public object favoriteMenu(DataTable dataUser)
        {
            int GroupId = Convert.ToInt32(dataUser.Rows[0]["ss_group_id"]);
            int PortfolioId = Convert.ToInt32(dataUser.Rows[0]["portfolio_id"]);
            string UserId = fn.DecryptString(dataUser.Rows[0]["user_id"].ToString());
            return favRepo.getMenuFavorite(PortfolioId, UserId);
        }

        public Output Logout(AuthLogin Model)
        {
            Output _result = new Output();
            try
            {
                HttpContextAccessor Context = new HttpContextAccessor();
                var Headers = Context.HttpContext.Request.Headers;
                string Token = string.Empty;
                foreach (var key in Headers.Keys)
                {
                    if (key.ToLower() == "token")
                    {
                        Token = Headers[key].ToString();
                    }
                }
                Model.UserLog = Tools.DecryptString(Model.UserLog);

                var UserSession = new UserSession();
                UserSession.user_id = Model.UserLog;
                UserSession.token = Token;
                UserSession.ip_address = Tools.GetIpAddress();
                authRepo.DeleteUserSession(UserSession);

                authRepo.UpdateUserLog(Model.UserLog, Tools.GetIpAddress(), Token);

                userFCMRepo.DeleteUserFCM(Token);

            }
            catch (Exception ex)
            {
                _result = Tools.Error(ex);
                //throw ex;
            }
            return _result;
        }

        //public async Task<Output> ForgotPassword(ForgotPassword Model)
        public async Task<Output> ForgotPassword(ForgotPassword Model)
        {
            Output _result = new Output();
            try
            {
                SsUser dataUser = authRepo.GetDataAuthByEmail(Model.Email);
                if (dataUser == null)
                {
                    throw new Exception("Your Email Is Not Valid.");
                }
                if (dataUser.is_inactive == "Y")
                {
                    throw new Exception("Your Account Is InActive. Please Contact Your Administrator.");
                }

                
                string GenOTP = EncryptionLibrary.KeyGenerator.GetUniqueNumber(6);
                string OPTEncryp = EncryptionLibrary.EncryptText(GenOTP);

                authRepo.UpdateOTP(OPTEncryp, Model.Email, dataUser.ss_user_id);

                await SendEmailForgotAsync(dataUser, GenOTP);

                _result.Message = "Please check Your email.";
            }
            catch (Exception ex)
            {
                _result = Tools.Error(ex);
            }
            return _result;
        }

        async Task SendEmailForgotAsync(SsUser dataUser,string OTP)
        {
            try
            {
                string BodyTemplateFunction = string.Empty;
                if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
                {
                    _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }
                string PathFile = Path.Combine(_environment.WebRootPath, "Template", "ForgotPassword.html");

                //if (dataHeader.form_type.ToUpper() == "FORM")
                //{
                //    PathFile = Path.Combine(PathRoot, "Template", "MasterCrudPostgresForm.txt");
                //}
                using (StreamReader reader = new StreamReader(PathFile))
                {
                    BodyTemplateFunction = reader.ReadToEnd();
                }
                BodyTemplateFunction = BodyTemplateFunction.Replace("{UserName}", dataUser.user_name);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{OTP}", OTP);

                EmailModel ModelEmail = new EmailModel();
                ModelEmail.to = dataUser.email;
                ModelEmail.subject = "Forgot Password";
                ModelEmail.body = BodyTemplateFunction;

                ModelEmail.user_id = EncryptionLibrary.EncryptText(dataUser.user_id);

                Output _result = await _emailSender.SendEmailAsync(ModelEmail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Output Validate(string OTP)
        {
            Output _result = new Output();
            Dictionary<string, object> DataUser = new Dictionary<string, object>();
            try
            {
                string OtpEncrypt = EncryptionLibrary.EncryptText(OTP);
                SsUser dataUser = authRepo.GetDataAuthByOTP(OtpEncrypt);
                if (dataUser == null)
                {
                    throw new Exception("Your OTP Is Not Valid.");
                }
                if (dataUser.is_inactive == "Y")
                {
                    throw new Exception("Your Account Is InActive. Please Contact Your Administrator.");
                }

                

                DataUser.Add("user_id", EncryptionLibrary.EncryptText(dataUser.user_id));
                _result.Data = DataUser;
            }
            catch (Exception ex)
            {
                throw ex;
                //_result = Tools.Error(ex);
            }
            return _result;
        }
    }
}
