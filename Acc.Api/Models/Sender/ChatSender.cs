using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class ChatSender
    {
        public string portfolio_id { get; set; }
        public string subportfolio_id { get; set; }
        public string subject { get; set; }
        public string user_id_from { get; set; }
        public string user_id_to { get; set; }
        public string doc_type { get; set; }
        public string doc_no { get; set; }
        public int? mk_quotation_id { get; set; }
        public int? op_order_id { get; set; }
        public int? mk_open_order_id { get; set; }
        public int current_page { get; set; }
        public string user_input { get; set; }
    }
    public class ChatDetail
    {
        public int ss_chat_d_id { get; set; }
        public int ss_chat_h_id { get; set; }
        public string chat_text { get; set; }
        public DateTime chat_date { get; set; }
        public string user_id_from { get; set; }
        public string user_id_to { get; set; }
        public string user_input { get; set; }
        public string user_edit { get; set; }
        public DateTime time_input { get; set; }
        public DateTime time_edit { get; set; }
    }
    public class GetChat
    {
        public int ss_chat_d_id { get; set; }
        public string chat_text { get; set; }
        public DateTime chat_date { get; set; }
        public string user_id_from { get; set; }
        public string user_name { get; set; }
    }
    public class ChatID
    {
        public int row_id { get; set; }
    }
}
