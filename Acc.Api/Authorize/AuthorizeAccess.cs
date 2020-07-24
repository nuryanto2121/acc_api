using Acc.Api.DataAccess;
using Acc.Api.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Authorize
{
    public class AuthorizeAccess
    {
        private string connectionString;
        private string _Message;
        private AuthRepo _AuthRepor;
        //public AuthorizeAccess(IConfiguration configuration)
        public AuthorizeAccess(string ConnectionString)
        {
            connectionString = ConnectionString;//Helper.ConnectionString(configuration);//
            _AuthRepor = new AuthRepo(connectionString);
        }

        public Output getAccessAPI(AuthorizationFilterContext context)
        {
            Output op = new Output();
            try
            {
                string IpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
                var Headers = context.HttpContext.Request.Headers;
                bool isToken = false;
                foreach (var key in Headers.Keys)
                {
                    if (key.ToLower() == "token")
                    {
                        if (!isAuthorize(Headers[key], IpAddress))
                        {
                            op.Error = true;
                            op.Message = _Message;
                        }
                        isToken = true;
                        //return op;
                    }
                    //else
                    //{
                    //    op.Error = true;
                    //    op.Message = "Invalid Token";
                    //    return op;
                    //}
                }

                if (!isToken)
                {
                    op.Error = true;
                    op.Message = "Invalid Token";
                }

            }
            catch (Exception ex)
            {
                op.Error = true;
                op.Message = ex.Message;
            }

            return op;
        }

        private bool isAuthorize(string Token, string IpAddress)
        {
            bool _result = true;
            try
            {
                var encodeString = Token;//values.First();//req.Request.Headers.GetValues("Token").First();
                if (string.IsNullOrEmpty(encodeString))
                {
                    _Message = "Invalid Token";
                    return false;
                }

                var Key = EncryptLibrary.AES256Encryption.EncryptionLibrary.DecryptText(encodeString);
                string[] Parts = Key.Split(new string[] { "~!@#$%^" }, StringSplitOptions.None);
                if (Parts.Count() < 6)
                {
                    _Message = "Invalid Token";
                    return false;
                }
                var UserID = Parts[0];
                var RandomKey = Parts[1];
                var Password = Parts[2];
                var Ip = Parts[3];
                long tik = long.Parse(Parts[4]);
                DateTime tokenCreate = new DateTime(tik);
                long tik2 = long.Parse(Parts[5]);
                DateTime ExpireOn = new DateTime(tik2);
                


                //if (Ip != IpAddress)
                //{
                //    _Message = "Invalid Ip Address";
                //    return false;
                //}

                var Auth = new AuthLogin();
                Auth.UserLog = UserID;
                Auth.PassLog = Password;
                var don = _AuthRepor.GetDataAuth(Auth);
                if (don == null || don.Rows.Count == 0)
                {
                    _Message = "Invalid User";
                    return false;
                }
                else
                {
                    UserSession uslog = _AuthRepor.GetDataSessionLog(Auth.UserLog, Token);
                    if (DateTime.Now > uslog.expire_on)
                    {
                        _Message = "Your Session Has Expired";
                        return false;
                    }

                }


            }
            catch (Exception ex)
            {
                _result = false;
                _Message = ex.Message;
                //throw (ex);
            }
            return _result;
        }
       
    }
}
