using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class SsGroupService
    {
        private FunctionString fn;
        private IConfiguration config;
        private SsGroupRepo SsGroupRepo;

        public SsGroupService(IConfiguration configuration)
        {
            fn = new FunctionString(Tools.ConnectionString(configuration));
            SsGroupRepo = new SsGroupRepo(Tools.ConnectionString(configuration));
        }

        public Output GetMenuJson(string portfolio_id, string group_id)
        {
            Output _result = new Output();
            try
            {
                int? ss_portfolio_id = portfolio_id.ToLower() == "null" ? 0 : Convert.ToInt16(fn.DecryptString(portfolio_id));
                int? ss_group_id = group_id.ToLower() == "null" ? 0 : Convert.ToInt16(fn.DecryptString(group_id));
                _result.Data = SsGroupRepo.GetMenuJson(ss_portfolio_id, ss_group_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output Save(VmSsGroup Model)
        {
            Output _result = new Output();
            try
            {
                Model.DataHeader.ss_portfolio_id = Convert.ToInt32(fn.DecryptString(Model.portfolio_id));
                Model.DataHeader.user_input = fn.DecryptString(Model.DataHeader.user_input);
                Model.DataHeader.user_edit = Model.DataHeader.user_input;

                SsGroup dtHeader = Model.DataHeader;
                var dtROw = SsGroupRepo.SaveHeader(dtHeader);

                Model.DataDetail.ForEach(delegate (SsMenuGroup dt)
                {
                    dt.ss_portfolio_id = Model.DataHeader.ss_portfolio_id;
                    dt.ss_group_id = dtROw.row_id;
                    dt.user_input = Model.DataHeader.user_input;
                    SsGroupRepo.SaveDetail(dt);
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output Update(VmSsGroup Model)
        {
            Output _result = new Output();
            try
            {
                Model.DataHeader.ss_portfolio_id = Convert.ToInt32(fn.DecryptString(Model.portfolio_id));
                Model.DataHeader.user_edit = fn.DecryptString(Model.DataHeader.user_edit);
                //Model.DataHeader.ss_group_id = Convert.ToInt32(fn.DecryptString(Model.group_id));

                SsGroup dtHeader = Model.DataHeader;
                var dtROw = SsGroupRepo.Update(dtHeader);

                SsGroupRepo.DeleteDetail(Model.DataHeader.ss_portfolio_id, Model.DataHeader.ss_group_id);
                Model.DataDetail.ForEach(delegate (SsMenuGroup dt)
                {
                    dt.ss_portfolio_id = Model.DataHeader.ss_portfolio_id;
                    dt.ss_group_id = dtHeader.ss_group_id;
                    dt.user_input = Model.DataHeader.user_edit;
                    SsGroupRepo.SaveDetail(dt);
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }


    }
}
