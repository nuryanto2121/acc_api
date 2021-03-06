using System;

namespace Acc.Driver.Api.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class ErrorStatusCode
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}