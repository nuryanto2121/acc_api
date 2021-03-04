using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class RespsoneMNC
    {
        [JsonProperty("certificateNo")]
        public string CertificateNo
        {
            get; set;
        }

        [JsonProperty("resultCd")]
        public string ResultCd
        {
            get; set;
        }

        [JsonProperty("resultMsg")]
        public string ResultMsg
        {
            get; set;
        }
    }
}
