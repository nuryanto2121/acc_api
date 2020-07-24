using Dapper;
using GenerateFunctionPostgres.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GenerateFunctionPostgres.ClassGenerateFunction
{
    public class FunctionPostgres
    {
        public IDbConnection DBconnection;
        private string connectionString;
        public FunctionPostgres(string ConnectionString)
        {
            DBconnection = new NpgsqlConnection(ConnectionString);
            connectionString = ConnectionString;
        }

        public List<MOptionLookUpPostgres> LookupGetListBy(string LookUpCd, string ColumnCd)
        {
            List<MOptionLookUpPostgres> dataList = new List<MOptionLookUpPostgres>();
            using (IDbConnection conn = DBconnection)
            {
                string sqlQuery = string.Format("SELECT ss_option_lookup_id,option_lookup_cd,column_db,view_name,source_field,source_where,user_input,user_edit,time_input,time_edit,display_lookup,is_lookup_list,is_asyn FROM  ss_option_lookup where option_lookup_cd = @OptionLookUpCd and column_db = @ColumnDB ");
                try
                {
                    conn.Open();
                    dataList = conn.Query<MOptionLookUpPostgres>(sqlQuery, new { OptionLookUpCd = LookUpCd, ColumnDB = ColumnCd }).ToList();

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
            return dataList;
        }
        public List<MOptionDB> OptionDBList(string OptionUrl)
        {
            List<MOptionDB> dataList = new List<MOptionDB>();
            using (IDbConnection conn = DBconnection)
            {
                string sqlQuery = string.Format(@"SELECT 
                                              ss_option_db_id,
                                              option_url,
                                              method_api,
                                              sp,
                                              line_no,
                                              table_name,
                                              user_input,
                                              user_edit,
                                              time_input,
                                              time_edit,
                                                method_vue,
                                                order_save,
                                                order_update
                                            FROM 
                                              public.ss_option_db 
                                            WHERE option_url iLike @option_url;");
                try
                {
                    conn.Open();
                    dataList = conn.Query<MOptionDB>(sqlQuery, new { option_url = OptionUrl }).ToList();

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
            return dataList;
        }
        public List<ParameterPostgres> GetResultFieldFunction(string FunctionName)
        {
            List<ParameterPostgres> tt = new List<ParameterPostgres>();
            using (IDbConnection conn = DBconnection)
            {
                try
                {
                    string DbName = GetString(connectionString, "Database=", ";");
                    string Query = @"   SELECT routines.routine_name as routine_name,
	                                        parameters.parameter_name as column_name, 
	                                        parameters.data_type,
	                                        parameters.ordinal_position as position
                                        FROM information_schema.routines
                                        LEFT JOIN information_schema.parameters ON routines.specific_name = parameters.specific_name
                                        WHERE routines.specific_catalog = @catalog 
                                        AND routines.specific_schema = 'public' 
                                        AND routines.routine_name = @routine_name 
                                        AND parameters.parameter_mode <> 'IN'
                                        ORDER BY routines.specific_name, routines.routine_name, parameters.ordinal_position; ";
                    conn.Open();
                    tt = conn.Query<ParameterPostgres>(Query, new { routine_name = FunctionName, catalog = DbName }).ToList();
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
            return tt;
        }
        public static string GetString(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                try
                {
                    return strSource.Substring(Start, End - Start);
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
            else
            {
                return null;
            }
        }
        public List<ParameterPostgres> getParameterFunction(string fn_name)
        {
            List<ParameterPostgres> tt = new List<ParameterPostgres>();
            using (IDbConnection conn = DBconnection)
            {
                try
                {
                    string Query = string.Format(@"select routine_name,parameter_name as column_name,
                                    data_type,oridinal_position as position from public.get_param_function('{0}');", fn_name);
                    conn.Open();
                    tt = conn.Query<ParameterPostgres>(Query).ToList();
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
            return tt;
        }

        public List<ParameterPostgres> GetPropertiesTable(string TableName)
        {
            List<ParameterPostgres> tt = new List<ParameterPostgres>();
            using (IDbConnection conn = DBconnection)
            {
                try
                {
                    string Query = @" select 
		                            table_name as routine_name,
   		                            column_name,
		                            data_type,        
		                            ordinal_position as position,
		                            case when character_maximum_length is not null
			                            then character_maximum_length
			                            else numeric_precision end as max_length,
		                            CASE WHEN is_nullable iLIKE 'no'
        	                            THEN 0
                                        ELSE 1 END::bit as is_nullable,
		                            numeric_precision as precision, numeric_scale as scale,
		                            COALESCE(column_default,'') as default_value
                            from information_schema.columns
                            where table_schema not in ('information_schema', 'pg_catalog')
                            AND table_name iLIKE @table_name
                            order by table_schema,table_name,ordinal_position;";
                    conn.Open();
                    tt = conn.Query<ParameterPostgres>(Query, new { table_name = TableName }).ToList();
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
            return tt;
        }

        public bool ExecQuery(string sQuery)
        {
            bool _result = false;
            using (IDbConnection conn = DBconnection)
            {
                try
                {
                    //string Query = string.Format("select routine_name,parameter_name as column_name,data_type,oridinal_position as position from public.get_param_function('{0}');", fn_name);
                    conn.Open();
                    conn.Query(sQuery);
                    _result = true;
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

    }
}
