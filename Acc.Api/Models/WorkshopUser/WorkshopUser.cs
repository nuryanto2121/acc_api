using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class MMWorkshopUser
    {
        [JsonProperty("descs")]
        public string Descs
        {
            get; set;
        }

        [JsonProperty("password")]
        public string Password
        {
            get; set;
        }

        [JsonProperty("phone_no")]
        public string PhoneNo
        {
            get; set;
        }

        [JsonProperty("file_name")]
        public string FileName
        {
            get; set;
        }

        [JsonProperty("path_file")]
        public string PathFile
        {
            get; set;
        }

        [JsonProperty("time_edit")]
        public DateTime TimeEdit
        {
            get; set;
        }

        [JsonProperty("user_edit")]
        public string UserEdit
        {
            get; set;
        }

        [JsonProperty("user_name")]
        public string UserName
        {
            get; set;
        }

        [JsonProperty("time_input")]
        public DateTime TimeInput
        {
            get; set;
        }

        [JsonProperty("user_input")]
        public string UserInput
        {
            get; set;
        }

        [JsonProperty("user_status")]
        public string UserStatus
        {
            get; set;
        }

        [JsonProperty("mm_workshop_id")]
        public int MmWorkshopId
        {
            get; set;
        }

        [JsonProperty("ss_portfolio_id")]
        public string SsPortfolioId
        {
            get; set;
        }

        [JsonProperty("mm_workshop_user_id")]
        public int MmWorkshopUserId
        {
            get; set;
        }
    }
}
