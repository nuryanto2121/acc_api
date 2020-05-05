using Acc.Api.Helper;
using Acc.Api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acc.Api.DataAccess
{
    public class OptionTemplateRepo
    {
        private string connectionString;
        IConfiguration config;
        public OptionTemplateRepo(IConfiguration Configuration)
        {
            config = Configuration;
            connectionString = Tools.ConnectionString(Configuration);            
        }
        public List<OptionFunction> GetListOptFunction(string OptionFunctionCd, string ModuleCd)
        {
            List<OptionFunction> tt = new List<OptionFunction>();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    string SqlQuery = "select ss_option_function_id,option_function_cd,module_cd,sp_name,sp_param,user_input,user_edit,time_input,time_edit from public.ss_option_function  where option_function_cd iLIKE @option_function_cd and module_cd iLIKE @module_cd  ";
                    conn.Open();
                    tt = conn.Query<OptionFunction>(SqlQuery, new { option_function_cd = OptionFunctionCd, module_cd = ModuleCd }).ToList();
                    //conn.Open();
                    //tt = conn.Query<OptionDB>("SELECT * FROM public.ss_option_function where UPPER(md5(option_url::varchar)) = @OptionUrl ", new { OptionUrl = OptionUrl.ToUpper() }).ToList();
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
        public DefineColumn GetDefineColumn(string user_id, string subportfolio_cd, string OptionSeq, int LineNo)
        {
            DefineColumn dd = null;// new DefineColumn();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    StringBuilder sQuery = new StringBuilder();
                    sQuery.AppendFormat("SELECT ");
                    sQuery.AppendFormat("subportfolio_id,Column_Field ");
                    sQuery.AppendFormat("FROM SS_Define_Column ");
                    sQuery.AppendFormat("WHERE user_id ='{0}' AND subportfolio_id = '{1}' ", user_id, subportfolio_cd);
                    sQuery.AppendFormat("AND option_url iLike '{0}' AND Line_No = '{1}' ", OptionSeq.ToUpper(), LineNo);

                    conn.Open();
                    dd = conn.Query<DefineColumn>(sQuery.ToString()).FirstOrDefault();
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
            return dd;
        }
        public OptionLookUp GetdataLookUp(string LookUpCd, string ColumnDB)
        {
            OptionLookUp op = new OptionLookUp();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    op = conn.Query<OptionLookUp>("SELECT * FROM public.ss_option_lookup where  option_lookup_cd iLIKE @LookUpCd AND column_db iLIKE @ColumnDb", new { LookUpCd = LookUpCd, ColumnDb = ColumnDB }).FirstOrDefault();
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
            return op;
        }
        public List<OptionDB> GetList(string OptionUrl)
        {
            List<OptionDB> op = new List<OptionDB>();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    op = conn.Query<OptionDB>("SELECT * FROM public.SS_Option_DB where UPPER(md5(option_url::varchar)) = @OptionUrl ", new { OptionUrl = OptionUrl.ToUpper() }).ToList();
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
            return op;
        }
        public List<OptionDB> GetListUrl(string OptionUrl)
        {
            List<OptionDB> op = new List<OptionDB>();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    op = conn.Query<OptionDB>("SELECT * FROM public.SS_Option_DB where option_url iLIKE @OptionUrl ", new { OptionUrl = OptionUrl.ToUpper() }).ToList();
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
            return op;
        }
        public List<ParamFunction> getListParamType(string fn_name)
        {
            List<ParamFunction> tt = new List<ParamFunction>();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    string[] bb = fn_name.Split('.');
                    fn_name = bb.Count() > 1 ? bb[1] : bb[0];
                    string Query = string.Format("select * from public.get_param_function('{0}');", fn_name);
                    conn.Open();
                    tt = conn.Query<ParamFunction>(Query).ToList();
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

        public List<FieldSource> getListFieldType(string fn_name, bool isViewFunctio = false)
        {
            List<FieldSource> tt = new List<FieldSource>();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    string[] bb = fn_name.Split('.');
                    fn_name = bb.Count() > 1 ? bb[1] : bb[0];
                    string sQuery = string.Empty;
                    string DbName = Tools.GetString(config.GetValue<string>("appSetting:ConnectionString"),"Database=",";");
                    if (isViewFunctio)
                    {
                        sQuery = string.Format(@"   SELECT routines.routine_name as table_name,
  	                                               parameters.parameter_name as column_name, 
  	                                               parameters.data_type,
  	                                               parameters.ordinal_position as position
		                                        FROM information_schema.routines
			                                        LEFT JOIN information_schema.parameters ON routines.specific_name = parameters.specific_name
		                                        WHERE
					                                        routines.specific_catalog = '{1}' AND
					                                        routines.specific_schema = 'public' AND
					                                        routines.routine_name = '{0}' AND
		                                              parameters.parameter_mode <> 'IN'
		                                        ORDER BY routines.specific_name, routines.routine_name, parameters.ordinal_position;", fn_name, DbName);
                    }
                    else
                    {
                        sQuery = string.Format(@"select table_schema,
                                                           table_name,
                                                           ordinal_position as position,
                                                           column_name,
                                                           data_type,
                                                           case when character_maximum_length is not null
                                                            then character_maximum_length
                                                            else numeric_precision end as max_length,
                                                            is_nullable,
                                                            numeric_precision as precision, numeric_scale as scale,
                                                           column_default as default_value
                                                    from information_schema.columns
                                                    where table_schema not in ('information_schema', 'pg_catalog')
                                                    AND table_name iLIKE '{0}'
                                                    order by table_schema, 
                                                             table_name,
                                                             ordinal_position;", fn_name);
                    }


                    conn.Open();
                    tt = conn.Query<FieldSource>(sQuery.ToString()).ToList();
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

        //public DefineColumn GetDefineColumn(string user_id, string subportfolio_short_name, string OptionUrl, int LineNo)
        //{
        //    DefineColumn dd = null;// new DefineColumn();
        //    using (IDbConnection conn = Tools.DBConnection(connectionString))
        //    {
        //        try
        //        {
        //            StringBuilder sQuery = new StringBuilder();
        //            sQuery.AppendFormat("SELECT ");
        //            sQuery.AppendFormat("subportfolio_short_name,Column_Field ");
        //            sQuery.AppendFormat("FROM SS_Define_Column ");
        //            sQuery.AppendFormat("WHERE user_id ='{0}' AND subportfolio_short_name = '{1}' ", user_id, subportfolio_short_name);
        //            sQuery.AppendFormat("AND (upper(md5(option_url::varchar)) = '{0}' OR option_url iLike '{0}') AND Line_No = '{1}' ", OptionUrl.ToUpper(), LineNo);

        //            conn.Open();
        //            dd = conn.Query<DefineColumn>(sQuery.ToString()).FirstOrDefault();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw (ex);
        //        }
        //        finally
        //        {
        //            if (conn.State == ConnectionState.Open) conn.Close();
        //        }
        //    }
        //    return dd;
        //}

        public bool InsertDefineColumn(string user_id, string p_subportfolio_id, string OptionUrl, int LineNo, string ColumnField)
        {
            bool _result = false;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    DynamicParameters spParam = new DynamicParameters();
                    string SpName = string.Empty;

                    SpName = "fss_define_column_i";
                    spParam.Add("@p_option_url", OptionUrl, dbType: DbType.String);


                    spParam.Add("@p_user_id", user_id, dbType: DbType.String);
                    spParam.Add("@p_subportfolio_id", Convert.ToInt32(p_subportfolio_id), dbType: DbType.Int32);
                    spParam.Add("@p_line_no", LineNo, dbType: DbType.Int32);
                    spParam.Add("@p_column_field", ColumnField, dbType: DbType.String);

                    conn.Query(SpName, spParam, commandType: CommandType.StoredProcedure);
                    _result = true;
                }
                catch (Exception ex)
                {
                    throw ex;
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
