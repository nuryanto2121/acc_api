using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class SsGroup
    {
        public int ss_portfolio_id { get; set; }
        public int ss_group_id { get; set; }
        public string descs { get; set; }
        public string short_descs { get; set; }
        public string user_type { get; set; }
        public string user_input { get; set; }
        public string user_edit { get; set; }
        public DateTime time_input { get; set; }
        public DateTime time_edit { get; set; }
        public int lastupdatestamp { get; set; }
    }
    public class VmSsGroup
    {
        public string ss_portfolio_id { get; set; }
        public string group_id { get; set; }
        public SsGroup DataHeader { get; set; }
        public List<SsMenuGroup> DataDetail { get; set; }
    }
}
