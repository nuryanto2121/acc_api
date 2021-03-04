using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Driver.Api.Models
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }
        public int JumlahFieldList { get; set; }
        public int TokenExpire { get; set; }
        public int IdleWeb { get; set; }
    }
}
