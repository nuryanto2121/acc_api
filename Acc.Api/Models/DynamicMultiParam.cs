using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class DynamicMultiParam
    {
        public string option_url { get; set; }
        public int line_no { get; set; }
        public List<JObject> Data { get; set; }
    }
    public class ExportExcelJson
    {
        public string Token { get; set; }
        public string Data { get; set; }
    }
    public partial class MultiPostGroupPermission
    {
        public string option_url { get; set; }
        public int line_no { get; set; }
        public string user_input { get; set; }
        public string subportfolio_short_name { get; set; }
        public string user_group { get; set; }
        public List<JObject> Data { get; set; }
    }
    public partial class MultiPostUserPermission
    {
        public string option_url { get; set; }
        public int line_no { get; set; }
        public string user_input { get; set; }
        public string subportfolio_short_name { get; set; }
        public string user_id { get; set; }
        public List<JObject> Data { get; set; }
    }
}
