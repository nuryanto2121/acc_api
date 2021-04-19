using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class SsPortfolio
    {
        [JsonProperty("city")]
        public string city
        {
            get; set;
        }

        [JsonProperty("name")]
        public string name
        {
            get; set;
        }

        [JsonProperty("email")]
        public string email
        {
            get; set;
        }

        [JsonProperty("fax_no")]
        public string fax_no
        {
            get; set;
        }

        [JsonProperty("address")]
        public string address
        {
            get; set;
        }

        [JsonProperty("post_cd")]
        public string post_cd
        {
            get; set;
        }

        [JsonProperty("remarks")]
        public string remarks
        {
            get; set;
        }

        [JsonProperty("website")]
        public string website
        {
            get; set;
        }

        [JsonProperty("phone_no")]
        public string phone_no
        {
            get; set;
        }

        [JsonProperty("gps_token")]
        public string gps_token
        {
            get; set;
        }

        [JsonProperty("time_edit")]
        public DateTime time_edit
        {
            get; set;
        }

        [JsonProperty("user_edit")]
        public string user_edit
        {
            get; set;
        }

        [JsonProperty("short_name")]
        public string short_name
        {
            get; set;
        }

        [JsonProperty("time_input")]
        public DateTime time_input
        {
            get; set;
        }

        [JsonProperty("user_input")]
        public string user_input
        {
            get; set;
        }

        [JsonProperty("reference_no")]
        public string reference_no
        {
            get; set;
        }

        [JsonProperty("gps_map_token")]
        public string gps_map_token
        {
            get; set;
        }

        [JsonProperty("rounding_factor")]
        public long rounding_factor
        {
            get; set;
        }

        [JsonProperty("ss_portfolio_id")]
        public long ss_portfolio_id
        {
            get; set;
        }

        [JsonProperty("is_prod_insurance")]
        public long is_prod_insurance
        {
            get; set;
        }

        [JsonProperty("picture_file_name")]
        public string picture_file_name
        {
            get; set;
        }

        [JsonProperty("merchant_id_funding")]
        public string merchant_id_funding
        {
            get; set;
        }

        [JsonProperty("reference_file_name")]
        public string reference_file_name
        {
            get; set;
        }
    }
}


