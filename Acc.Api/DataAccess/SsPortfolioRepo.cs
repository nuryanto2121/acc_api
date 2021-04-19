using Acc.Api.Enum;
using Acc.Api.Helper;
using Acc.Api.Interface;
using Acc.Api.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.DataAccess
{
    public class SsPortfolioRepo : IRepository<SsPortfolio, int>
    {
        private string connectionString;
        private FunctionString fn;
        public SsPortfolioRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
            fn = new FunctionString(ConnectionString);
        }
        public bool Delete(int key, int timestamp)
        {
            throw new NotImplementedException();
        }

        public SsPortfolio GetById(int key, int timestamp)
        {
            SsPortfolio t = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string strQuery = @"SELECT  *
                        FROM public.ss_portfolio  WHERE ss_portfolio_id = @ss_portfolio_id";
                try
                {
                    conn.Open();
                    t = conn.Query<SsPortfolio>(strQuery, new { ss_portfolio_id = key }).SingleOrDefault();
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }

            }

            return t;
        }

        public List<SsPortfolio> GetList()
        {
            throw new NotImplementedException();
        }

        public List<SsPortfolio> GetList(int pageSize, int currentPage, string sortName, string sortOrder, string Parameter)
        {
            throw new NotImplementedException();
        }

        public bool Save(SsPortfolio domain)
        {
            throw new NotImplementedException();
        }

        public object SelectScalar(SQL.Function.Aggregate function, string column, string ParamWHere = null)
        {
            throw new NotImplementedException();
        }

        public bool Update(SsPortfolio domain)
        {
            throw new NotImplementedException();
        }
    }
}
