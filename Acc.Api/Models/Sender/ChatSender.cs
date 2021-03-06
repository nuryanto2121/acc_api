﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class ChatHeader
    {
        public int ss_chat_h_id { get; set; }
        public int ss_portfolio_id { get; set; }
        public int ss_subportfolio_id { get; set; }
        public string subject { get; set; }
        public string user_id_to { get; set; }
        public DateTime doc_date { get; set; }
        public string doc_type { get; set; }
        public string doc_no { get; set; }
        public string user_id_from { get; set; }
        public int row_id { get; set; }
        public string option_url { get; set; }
        public int line_no { get; set; }
        public string url_view_detail { get; set; }
       // public string name_user_chat { get; set; }
    }
    public class ChatSender
    {
        public string portfolio_id { get; set; }
        public string subportfolio_id { get; set; }
        public string subject { get; set; }
        public string user_id_from { get; set; }
        public string user_id_to { get; set; }
        public string doc_type { get; set; }
        public string doc_no { get; set; }
        [Required]
        public int? row_id { get; set; }
        public string option_url { get; set; }
        public string url_view_detail { get; set; }
        public int line_no { get; set; }
        //public int? mk_quotation_id { get; set; }
        //public int? op_order_id { get; set; }
        //public int? mk_open_order_id { get; set; }
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
        public bool is_file { get; set; } = false;
        public int? ss_chat_attachment_id { get; set; }
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
        public bool is_file { get; set; }
        public int ss_chat_attachment_id { get; set; }
        public string file_name { get; set; }
        public string file_type { get; set; }
        public string path_file { get; set; }

    }
    public class RowID
    {
        public int row_id { get; set; }

        public static implicit operator int(RowID v)
        {
            throw new NotImplementedException();
        }
    }
    public class ChatListUser
    {
        public int ss_chat_h_id { get; set; }
        public int ss_portfolio_id { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public bool is_admin { get; set; }
    }
    public class ChatAttachment
    {
        public int ss_chat_h_id { get; set; }
        public int ss_portfolio_id { get; set; }
        public DateTime chat_date { get; set; }
        public string file_name { get; set; }
        public string file_type { get; set; }
        public string path_file { get; set; }
    }

    public class ChatNotif
    {
        public int v_chat { get; set; }
        public int v_notif { get; set; }
    } 
}
