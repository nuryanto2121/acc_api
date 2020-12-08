using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class EasyGoLastPositionParam
    {
        [JsonProperty("op_order_id")]
        public int OpOrderID { get; set; }

        [JsonProperty("easy_go_close_do")]
        public EasyGoLastPosition EasyGoLastPosition { get; set; }
    }
    public class EasyGoLastPosition
    {
        [JsonProperty("list_nopol")]
        public string[] ListNopol { get; set; }

        [JsonProperty("status_vehicle")]
        public long? StatusVehicle { get; set; }

        [JsonProperty("geo_code")]
        public string[] GeoCode { get; set; }
    }
}
