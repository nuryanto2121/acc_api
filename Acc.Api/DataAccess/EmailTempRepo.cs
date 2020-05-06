using Acc.Api.Helper;
using Acc.Api.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.DataAccess
{
    public class EmailTempRepo
    {
        private string connectionString;
        public EmailTempRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
        }
        public bool Save(EmailModelDB domain)
        {
            bool result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                string sqlQuery = @"INSERT INTO 
                                      public.ss_send_email
                                    (
                                      ss_portfolio_id,                         ss_subportfolio_id,
                                      sfrom,                                   sto,
                                      cc,                                      subject,
                                      body,                                    doc_type,
                                      doc_no,                                  user_input,
                                      user_edit,                               time_input,
                                      time_edit
                                    )
                                    VALUES (
                                      @ss_portfolio_id,                        @ss_subportfolio_id,
                                      @sfrom,                                  @sto,
                                      @cc,                                     @subject,
                                      @body,                                   @doc_type,
                                      @doc_no,                                 @user_input,
                                      @user_edit,                              @time_input,
                                      @time_edit
                                    );";
                try
                {
                    conn.Open();
                    conn.Execute(sqlQuery, domain);
                    result = true;
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
            return result;
        }
    }
}
