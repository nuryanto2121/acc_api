using Acc.Api.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class EmailModel
    {
        public string portfolio_id { get; set; }
        public string subportfolio_id { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string cc { get; set; }
        public string subject { get; set; }
        public string attachment_string { get; set; }
        public string path_attachment { get; set; }
        public string body { get; set; }
        public string doc_type { get; set; }
        public string doc_no { get; set; }
        public string user_id { get; set; }
    }
    public class EmailModelDB
    {
        public int ss_portfolio_id { get; set; }
        public int ss_subportfolio_id { get; set; }
        public string sfrom { get; set; }
        public string sto { get; set; }
        public string cc { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public string doc_type { get; set; }
        public string doc_no { get; set; }
        public string user_input { get; set; }
        public string user_edit { get; set; }
        public DateTime time_input { get; set; }
        public DateTime time_edit { get; set; }

        public static implicit operator EmailModelDB(EmailModel v)
        {
            EmailModelDB dd = new EmailModelDB();
            dd.ss_portfolio_id = Convert.ToInt32(Tools.DecryptString(v.portfolio_id));
            dd.ss_subportfolio_id = Convert.ToInt32(Tools.DecryptString(v.subportfolio_id));
            dd.sfrom = v.from;
            dd.sto = v.to;
            dd.cc = v.cc;
            dd.subject = v.subject;
            dd.body = v.body;
            dd.doc_type = v.doc_type;
            dd.doc_no = v.doc_no;
            dd.user_input = Tools.DecryptString(v.user_id);
            dd.user_edit = Tools.DecryptString(v.user_id);
            dd.time_edit = DateTime.Now;
            dd.time_input = DateTime.Now;
            return dd;
        }
    }
}
