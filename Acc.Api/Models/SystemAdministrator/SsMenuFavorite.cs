using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Models.SystemAdministrator
{
    public class SsMenuFavorite : BaseEntity
    {
        public int ss_user_favorite_id { get; set; }
        public int ss_portfolio_id { get; set; }
        public int group_id { get; set; }
        public string user_id { get; set; }
        public int ss_menu_id { get; set; }
        public string user_input { get; set; }
        public string User_edit { get; set; }
        public DateTime time_input { get; set; }
        public DateTime time_edit { get; set; }
    }

    public class ParamMenuFav
    {
        public int ss_menu_id { get; set; }
        public string user_id { get; set; }
        public string portfolio_id { get; set; }
    }
}
