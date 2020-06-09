using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class DashboardService
    {
        private FunctionString fn;
        private IConfiguration config;
        private DynamicService dynamicService;
        private string connectionString;
        public DashboardService(IConfiguration configuration)
        {
            fn = new FunctionString(Tools.ConnectionString(configuration));
            dynamicService = new DynamicService(configuration);
            connectionString = Tools.ConnectionString(configuration);
        }

        public Output Admin(string PortfolioID, string UserID)
        {
            var _result = new Output();
            Dictionary<string, object> ObjOutput = new Dictionary<string, object>();
            try
            {
                string MvSpName = string.Empty;
                int iStart = 1;
                int iPerPage = 10;
                string sSortField = "order by time_edit DESC";
                string sWhere = string.Empty;
                string allCoulumn = "*";
                int portfolio_id = Convert.ToInt32(fn.DecryptString(PortfolioID));
                string user_id = fn.DecryptString(UserID);


                //fleet master 
                sWhere = string.Empty;//string.Format("portfolio_id={0}", portfolio_id);
                MvSpName = "vfm_fleet_mstr_list";
                var dataFleetMaster = dynamicService.QueryList(MvSpName, iStart, iPerPage, sSortField, sWhere, allCoulumn);

                // driver mgm
                MvSpName = "vfm_driver";
                sWhere = string.Empty; //string.Format("portfolio_id={0}", portfolio_id);
                var dataDrivermgm = dynamicService.QueryList(MvSpName, iStart, iPerPage, sSortField, sWhere, allCoulumn);

                //User mgm
                MvSpName = "vss_user";
                sWhere = string.Format("portfolio_id={0}", portfolio_id);
                var dataUsermgm = dynamicService.QueryList(MvSpName, iStart, iPerPage, sSortField, sWhere, allCoulumn);
                // chart
                MvSpName = "vss_user_mgm_chart";
                sWhere = string.Format("ss_portfolio_id={0}", portfolio_id);
                var dataUserChart = dynamicService.QueryList(MvSpName, sWhere);
                

                ObjOutput.Add("data_fleet", dataFleetMaster);
                ObjOutput.Add("data_driver_mgm", dataDrivermgm);
                ObjOutput.Add("data_user_mgm", dataUsermgm);
                ObjOutput.Add("data_user_mgm_status", dataUserChart);
                _result.Data = ObjOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        
    }
}
