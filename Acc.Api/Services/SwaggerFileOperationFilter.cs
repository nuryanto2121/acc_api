using Acc.Api.Authorize;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.OperationId == "Post")
            {
                operation.Parameters = new List<IParameter>
                {
                    new NonBodyParameter
                    {
                        Name = "myFile",
                        Required = true,
                        Type = "file",
                        In = "formData"
                    }
                };

                if (operation.Parameters == null)
                    operation.Parameters = new List<IParameter>();

                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "Token",
                    In = "header",
                    Type = "string",
                    Required = true // set to false if this is optional
                });
            }


        }
    }

    public class AddAuthorizationHeader : IOperationFilter
    {
        void IOperationFilter.Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IParameter>();
            }

            var authorizeAttributes = context.ApiDescription
                .ControllerAttributes()
                .Union(context.ApiDescription.ActionAttributes())
                .OfType<AuthorizeAttribute>();
            var allowAnonymousAttributes = context.ApiDescription.ActionAttributes().OfType<AllowAnonymousAttribute>();

            if (!authorizeAttributes.Any() && !allowAnonymousAttributes.Any())
            {
                return;
            }

            var parameter = new NonBodyParameter
            {
                Name = "Token",
                In = "header",
                Description = "The bearer token",
                Required = true,
                Type = "string"
            };

            operation.Parameters.Add(parameter);
        }
    }
    public class MyHeaderFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            //var authorizeAttributes = context.ApiDescription
            //  .ControllerAttributes()
            //  .Union(context.ApiDescription.ActionAttributes())
            //  .OfType<APIAuthorizeAttribute>();

            //var allowAnonymousAttributes = context.ApiDescription.ActionAttributes().OfType<AllowAnonymousAttribute>();//context.ApiDescription.ActionAttributes().OfType<AllowAnonymousAttribute>();
            //var ddd = context.MethodInfo.CustomAttributes.OfType<AllowAnonymousAttribute>().Any();
            //var yyy = context.MethodInfo.CustomAttributes.OfType<APIAuthorizeAttribute>().Any();
            var dta = context.MethodInfo.CustomAttributes.ToList();

            bool isHeader = false;
            foreach (var aa in dta)
            {
                var ddddd = aa.AttributeType.Attributes;
                if (aa.AttributeType.ToString() == "Acc.Api.Authorize.APIAuthorizeAttribute")
                {
                    isHeader = true;
                }

            }


            if (!isHeader)
            {
                return;
            }

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "Token",
                In = "header",
                Type = "string",
                Required = true // set to false if this is optional
            });
        }
    }
}
