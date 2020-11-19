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
    public class TrackingRepo
    {
        private string connectionString;
        private FunctionString Fn;
        public TrackingRepo(string ConnectionString)
        {
            connectionString = ConnectionString;
            Fn = new FunctionString(connectionString);
        }

        public object GetDataTracking(int ID)
        {
            object _result = new object();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {

                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();

                    Parameters.Add("p_row_id", ID, dbType: DbType.Int32);

                    Parameters.Add("p_lastupdatestamp", 0, dbType: DbType.Int32);

                    var dd = conn.Query<dynamic>("fop_order_s", Parameters, commandType: CommandType.StoredProcedure);
                    _result = dd;
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
            return _result;
        }
        public RowID GetRowID(string OrderNo)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                RowID _result = new RowID();
                try
                {
                    conn.Open();
                    DynamicParameters Parameters = new DynamicParameters();
                    Parameters.Add("p_order_no", OrderNo);
                    _result = conn.Query<RowID>("get_tracking_op_order_id", Parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }

                return _result;
            }
        }


    }
}
