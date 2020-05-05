using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models
{
    public class Output : BaseEntity
    {
        public int Status { get; set; } = 200;
        public Boolean Error { get; set; } = false;
        public object Data { get; set; }
        public string Message { get; set; }
    }
    public class DTResultListDyn<T> : BaseEntity
    {
        public int Total { get; set; }
        public int Current_Page { get; set; }
        public int Last_Page { get; set; }
        public List<T> Data { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
        public string DefineColumn { get; set; }
        public string ExportToken { get; set; }
        public string AllColumn { get; set; }
        public string DefineSize { get; set; }
    }

    public class DTResultListLookup<T> : BaseEntity
    {
        public int Total { get; set; }
        public int Current_Page { get; set; }
        public int Last_Page { get; set; }
        public List<T> Data { get; set; }
        public string AllColumn { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
    }
}
