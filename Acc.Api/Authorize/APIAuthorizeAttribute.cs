using Acc.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Acc.Api.Authorize
{
    public class APIAuthorizeAttribute : TypeFilterAttribute
    {
        public APIAuthorizeAttribute() : base(typeof(APIAuthorizeFilter))
        {
            //Arguments = new object[] { new Claim(claimType, claimValue) };
            //Arguments = new object[] { new Claim(claimType, claimValue) };
        }
    }
    public class APIAuthorizeFilter : IAuthorizationFilter
    //public class APIAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        //private AuthorizeAccess _authorizeAccess = new AuthorizeAccess(Microsoft.Extensions.Configuration.IConfiguration);
        private string _message = string.Empty;
        private string _connectionString;
        private AuthorizeAccess _authorizeAccess;

        //public APIAuthorizeFilter(AuthorizationFilterContext context)
        //{
        //    var services = context.HttpContext.RequestServices;
        //    var settings = services.GetService(typeof(BHConfiguration));
        //    //_authorizeAccess = new AuthorizeAccess(settings.);
        //}

        public void OnAuthorization(AuthorizationFilterContext context)
        {

            var services = context.HttpContext.RequestServices;
            var settings = services.GetService(typeof(AppSettings));
            Type myType = settings.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

            foreach (PropertyInfo prop in props)
            {
                //object propValue = prop.GetValue(settings, null);
                if ("ConnectionString" == prop.Name)
                {
                    _connectionString = prop.GetValue(settings, null).ToString();
                }
            }


            if (!Authorize(context))
            {
                //context.Result = new ForbidResult();

                //context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized);
                context.Result = new JsonResult("")
                {
                    //StatusCode = System.Net.HttpStatusCode.Unauthorized
                    Value = new
                    {
                        Status = HttpStatusCode.Unauthorized,
                        Error = true,
                        Data = "",
                        Message = _message

                    },
                    StatusCode = Convert.ToInt32(HttpStatusCode.Unauthorized)
                };
                return;

            }
        }

        private bool Authorize(AuthorizationFilterContext context)
        {
            bool _result = false;
            try
            {
                _authorizeAccess = new AuthorizeAccess(_connectionString);
                var Op = _authorizeAccess.getAccessAPI(context);
                if (Op.Error)
                {
                    _message = Op.Message;
                    return false;
                }
                _result = true;
            }
            catch (Exception ex)
            {
                _result = false;
            }

            return _result;
        }

    }
}
