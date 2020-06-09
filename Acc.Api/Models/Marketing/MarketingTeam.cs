using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class MarketingTeam
    {
        //[JsonProperty("")]
        public string marketing_id { get; set; }
        public string name { get; set; }
        public DateTime join_date { get; set; }
        public int monthly_point { get; set; }
        public int monthly_new_prospect { get; set; }
        public bool is_my_team { get; set; }
    }

    public class MarketingTeamParam
    {
        public string user_id { get; set; }
        public string portfolio_id { get; set; }
        public List<MarketingTeam> data_team { get; set; }
    }
}
