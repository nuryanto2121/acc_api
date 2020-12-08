using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{

    public class EasyGoCloasParam
    {
        [JsonProperty("op_order_id")]
        public int OpOrderID { get; set; }

        [JsonProperty("easy_go_close_do")]
        public EasyGoClose EasyGoCloseDo { get; set; }
    }
    public class EasyGoClose
    {
        [JsonProperty("tgl_pod")]
        public DateTimeOffset TglPod { get; set; }

        [JsonProperty("tgl_closed")]
        public DateTimeOffset TglClosed { get; set; }

        [JsonProperty("ket_close")]
        public string KetClose { get; set; }

        [JsonProperty("photo_pod")]
        public string PhotoPod { get; set; }
    }
}
