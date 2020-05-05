using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Acc.Api.Helper
{
    public class HttpActionResult : IActionResult
    {
        private readonly JObject _message;
        private readonly HttpStatusCode _statusCode;

        public HttpActionResult(HttpStatusCode statusCode, JObject message)
        {
            _statusCode = statusCode;
            _message = message;
        }

        //public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(JsonConvert.SerializeObject(_message))
            };
            return Task.FromResult(response);
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            //throw new NotImplementedException();
            HttpResponseMessage response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(JsonConvert.SerializeObject(_message))
            };
            return Task.FromResult(response);
        }

    

    }
}

