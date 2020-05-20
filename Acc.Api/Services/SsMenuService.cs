using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Interface;
using Acc.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class SsMenuService : ICrudService<SsMenu, int>
    {
        private SsMenuRepo SsMenuRepo;
        private FunctionString fn;
        public SsMenuService(IConfiguration configuration)
        {
            SsMenuRepo = new SsMenuRepo(Tools.ConnectionString(configuration));
            fn = new FunctionString(Tools.ConnectionString(configuration));
        }

        public Output Delete(int Key, int Timestamp)
        {
            Output _result = new Output();
            try
            {
                // insert to temp then delelte level 1
                SsMenuRepo.SaveTempBydIdMenu(Key, "DELETE");
                SsMenuRepo.Delete(Key);

                List<SsMenu> ListMenuLvel2 = new List<SsMenu>();
                string ParamWHere = string.Format("WHERE parent_menu_id = {0}", Key);
                ListMenuLvel2 = SsMenuRepo.GetList(ParamWHere);
                ListMenuLvel2.ForEach(delegate (SsMenu data)
                {

                    SsMenuRepo.SaveTempBydIdMenu(data.ss_menu_id, "DELETE");
                    SsMenuRepo.Delete(data.ss_menu_id);

                    List<SsMenu> ListMenuLvel3 = new List<SsMenu>();
                    ParamWHere = string.Format("WHERE parent_menu_id = {0}", data.ss_menu_id);
                    ListMenuLvel3 = SsMenuRepo.GetList(ParamWHere);
                    ListMenuLvel3.ForEach(delegate (SsMenu data3)
                    {
                        SsMenuRepo.SaveTempBydIdMenu(data3.ss_menu_id, "DELETE");
                        SsMenuRepo.Delete(data3.ss_menu_id);
                    });

                });


                _result.Message = "Data Has been Deleted Successfuly.";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output GetDataBy(int Key, int Timestamp)
        {
            Output _result = new Output();
            try
            {
                _result.Data = SsMenuRepo.GetById(Key, Timestamp);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output GetMenuJson(string portfolio_id, string group_id)
        {
            Output _result = new Output();
            try
            {
                int? ss_portfolio_id = portfolio_id.ToLower() == "null" ? 0 : Convert.ToInt16(fn.DecryptString(portfolio_id));
                int? ss_group_id = group_id.ToLower() == "null" ? 0 : Convert.ToInt16(fn.DecryptString(group_id));
                _result.Data = SsMenuRepo.GetMenuJson(ss_portfolio_id, ss_group_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }


        public DTResultListDyn<dynamic> GetList(ParamList JModel)
        {
            throw new NotImplementedException();
        }

        public Output Insert(SsMenu Model)
        {
            Output _result = new Output();
            try
            {
                string sWhere = string.Format("parent_menu_id = {0}", Model.parent_menu_id);
                int OrderSeq = Convert.ToInt32(fn.SelectScalar(Enum.SQL.Function.Aggregate.Max, "ss_menu", "order_seq", sWhere)) + 1;
                Model.order_seq = OrderSeq;
                Model.user_input = Tools.DecryptString(Model.user_input);
                Model.User_edit = Model.user_input;
                Model.time_edit = DateTime.Now;
                Model.time_input = DateTime.Now;
                SsMenuRepo.Save(Model);
                _result.Message = "Data Has been Insert Successfuly.";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output Update(SsMenu Model)
        {
            Output _result = new Output();
            try
            {
                Model.time_edit = DateTime.Now;
                Model.User_edit = Tools.DecryptString(Model.User_edit);

                var ModelTemp = SsMenuRepo.GetById(Model.ss_menu_id);
                SsMenuTemp menuTemp = new SsMenuTemp();
                menuTemp.ss_menu_id = Model.ss_menu_id;
                menuTemp.title = ModelTemp.title;
                menuTemp.menu_url = ModelTemp.menu_url;
                menuTemp.menu_type = ModelTemp.menu_type;
                menuTemp.parent_menu_id = ModelTemp.parent_menu_id;
                menuTemp.icon_class = ModelTemp.icon_class;
                menuTemp.order_seq = ModelTemp.order_seq;
                menuTemp.ss_module_id = ModelTemp.ss_module_id;
                menuTemp.level_no = ModelTemp.level_no;
                menuTemp.on_event = "UPDATE";
                menuTemp.user_input = Model.User_edit;
                menuTemp.time_input = DateTime.Now;

                SsMenuRepo.SaveTemp(menuTemp);




                SsMenuRepo.Update(Model);
                _result.Message = "Data Has been Update Successfuly.";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public JObject GetMenuList()
        {
            JObject _result = new JObject();
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output GetDataList(string Parameter)
        {
            Output _result = new Output();
            try
            {
                Parameter = !string.IsNullOrEmpty(Parameter) ? "WHERE " + Parameter : string.Empty;
                //string sWhere = string.Format(@"WHERE parent_menu_id = {0} AND level_no = {1} {2}", Model.parent_menu_id, Model.level_no, Parameter);
                _result.Data = SsMenuRepo.GetList(Parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
    }
}
