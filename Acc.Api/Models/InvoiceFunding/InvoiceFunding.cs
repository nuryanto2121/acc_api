using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class ParamInvoiceFunding
    {
        [JsonProperty("ss_portfolio_id")]
        public int SsPortfolioID
        {
            get; set;
        }
        [JsonProperty("invoice_no")]
        public string InvoiceNo
        {
            get; set;
        }
        [JsonProperty("token")]
        public string Token
        {
            get; set;
        }
    }
}
