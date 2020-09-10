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
    public class MarketingTeamService
    {
        private FunctionString fn;
        private IConfiguration config;
        private DynamicService dynamicService;
        private MarketingTeamRepo marketingRepo;
        private string connectionString;

        public MarketingTeamService(IConfiguration configuration)
        {
            fn = new FunctionString(Tools.ConnectionString(configuration));
            dynamicService = new DynamicService(configuration);
            connectionString = Tools.ConnectionString(configuration);
            marketingRepo = new MarketingTeamRepo(configuration);
        }

        public Output GetLIst(string UserID, string PortfolioID)
        {
            var _result = new Output();
            try
            {
                UserID = fn.DecryptString(UserID);
                int ss_porfolio_id = Convert.ToInt32(fn.DecryptString(PortfolioID));
                _result.Data = marketingRepo.GetList(UserID, ss_porfolio_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output Save(MarketingTeamParam Model)
        {
            var _result = new Output();
            try
            {
                string UserId = fn.DecryptString(Model.user_id);
                int ss_porfolio_id = Convert.ToInt32(fn.DecryptString(Model.portfolio_id));
                //delete mk_marketing_team
                marketingRepo.DeleteDetail(ss_porfolio_id, UserId);

                Model.data_team.ForEach(delegate (MarketingTeam data)
                {
                    //if (data.is_my_team)
                    //{
                        marketingRepo.Insert(ss_porfolio_id, UserId, data.marketing_id, data.monthly_point, data.monthly_new_prospect,data.is_my_team);
                    //}
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
