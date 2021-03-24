using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class ParamCoaXml
    {
        [JsonProperty("ss_portfolio_id")]
        public string SsPortfolioId
        {
            get;set;
        }
        [JsonProperty("user_input")]
        public string UserInput
        {
            get;set;
        }
    }
}
