using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Models;
using Acc.Api.Models.SystemAdministrator;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class SsMenuFavoriteService
    {
        private SysMenuFavoriteRepo favRepo;
        private FunctionString fn;
        public SsMenuFavoriteService(IConfiguration configuration)
        {
            favRepo = new SysMenuFavoriteRepo(Tools.ConnectionString(configuration));
            fn = new FunctionString(Tools.ConnectionString(configuration));
        }

        public Output Insert(ParamMenuFav Model)
        {
            Output _result = new Output();
            try
            {
                //
                SsMenuFavorite dataFav = new SsMenuFavorite();
                dataFav.ss_menu_id = Model.ss_menu_id;
                dataFav.ss_portfolio_id = Convert.ToInt32(fn.DecryptString(Model.portfolio_id));
                dataFav.user_id = fn.DecryptString(Model.user_id);
                dataFav.user_input = dataFav.user_id;
                dataFav.User_edit = dataFav.user_id;
                dataFav.time_edit = DateTime.Now;
                dataFav.time_input = DateTime.Now;
                favRepo.Save(dataFav);

                _result.Data = favRepo.getMenuFavorite(dataFav.ss_portfolio_id, dataFav.user_id);

                _result.Message = "Data Has been Insert Successfuly.";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output Delete(int id)
        {
            Output _result = new Output();
            try
            {
                SsMenuFavorite dataFav = favRepo.GetById(id);
               

                favRepo.Delete(dataFav);

                _result.Data = favRepo.getMenuFavorite(dataFav.ss_portfolio_id, dataFav.user_id);

                _result.Message = "Data Has been Deleted Successfuly.";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
    }
}
