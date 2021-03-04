using EncryptLibrary.AES256Encryption;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using Dapper;
using Acc.Api.Models;
using NpgsqlTypes;
using Acc.Api.Enum;

namespace Acc.Api.Helper
{
    public class FunctionString
    {
        private string connectionstring;
        public FunctionString(string ConnectionString)
        {
            connectionstring = ConnectionString;
        }
        public object SelectScalar(SQL.Function.Aggregate function, string table, string column, string parameters)
        {
            object _result = null;
            using (IDbConnection conn = Tools.DBConnection(connectionstring))
            {
                StringBuilder sbQuery = new StringBuilder();
                //parameters = !string.IsNullOrEmpty(parameters) ? " WHERE " + parameters : parameters;
                if (!string.IsNullOrEmpty(parameters))
                {
                    parameters = parameters.Contains("WHERE") ? parameters : " WHERE " + parameters;
                }
                switch (function)
                {
                    case SQL.Function.Aggregate.Max:
                        sbQuery.AppendFormat("SELECT MAX({0}) FROM {1}  {2}", column, table, parameters);
                        break;
                    case SQL.Function.Aggregate.Min:
                        sbQuery.AppendFormat("SELECT MIN({0}) FROM {1}  {2}", column, table, parameters);
                        break;
                    case SQL.Function.Aggregate.Distinct:
                        sbQuery.AppendFormat("SELECT DISTINCT({0}) FROM {1}  {2}", column, table, parameters);
                        break;
                    case SQL.Function.Aggregate.Count:
                        sbQuery.AppendFormat("SELECT COUNT({0}) FROM {1}  {2}", column, table, parameters);
                        break;
                    case SQL.Function.Aggregate.Sum:
                        sbQuery.AppendFormat("SELECT SUM({0}) FROM {1}  {2}", column, table, parameters);
                        break;
                    case SQL.Function.Aggregate.Avg:
                        sbQuery.AppendFormat("SELECT AVG({0}) FROM {1}  {2}", column, table, parameters);
                        break;
                    default:
                        // do nothing 
                        break;
                }

                try
                {
                    conn.Open();
                    _result = conn.ExecuteScalar(sbQuery.ToString());
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
        public DataTable ToDataTable(List<dynamic> list)
        {

            DataTable dataTable = new DataTable();
            bool t = false;
            try
            {
                if (list.Count != 0)
                {
                    foreach (var x in list.ToList())
                    {
                        var item = x as IDictionary<string, object>;
                        var itemKey = item.Keys;
                        int i = 0;

                        object[] values = new object[itemKey.Count];

                        foreach (var info in itemKey)
                        {

                            if (!t)
                            {

                                dataTable.Columns.Add(info);

                            }
                            if (info.ToUpper() == "LASTUPDATESTAMP")
                            {
                                values[i] = item[info];

                            }
                            else
                            {

                                values[i] = item[info];
                            }

                            i += 1;
                            //dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
                        }
                        t = true;
                        dataTable.Rows.Add(values);
                    }
                }

            }
            catch (Exception ex)
            {
                throw (ex);
            }


            return dataTable;
        }

        public DataTable DataClearEncrypt(DataTable data)
        {
            if (data.Columns["user_id"] != null)
            {
                var user_id = EncryptionLibrary.EncryptText(data.Rows[0]["user_id"].ToString());
                //if (data.Columns["password"] != null)
                //{
                //    data.Columns.Remove("password");
                //}

                data.Rows[0].SetField("user_id", user_id);
            }
            if (data.Columns["subportfolio_id"] != null)
            {
                var subportfolio_id = EncryptionLibrary.EncryptText(data.Rows[0]["subportfolio_id"].ToString());
                data.Rows[0].SetField("subportfolio_id", subportfolio_id);
            }
            if (data.Columns["portfolio_id"] != null)
            {
                var portfolio_id = EncryptionLibrary.EncryptText(data.Rows[0]["portfolio_id"].ToString());
                data.Rows[0].SetField("portfolio_id", portfolio_id);
            }

            if (data.Columns["ss_group_id"] != null)
            {
                var portfolio_id = EncryptionLibrary.EncryptText(data.Rows[0]["ss_group_id"].ToString());
                data.Rows[0].SetField("ss_group_id", portfolio_id);
            }

            return data;
        }

        public JObject DecryptData(JObject JObj)
        {
            if (JObj["portfolio_id"] != null)
            {
                var ss_portfolio_id = JObj["portfolio_id"].ToString();
                if (isBase64(JObj["portfolio_id"].ToString()))
                {
                    ss_portfolio_id = EncryptionLibrary.DecryptText(JObj["portfolio_id"].ToString());
                }

                JObj["portfolio_id"] = ss_portfolio_id;
            }
            if (JObj["ss_group_login"] != null)
            {
                var ss_group_login = JObj["ss_group_login"].ToString();
                if (isBase64(JObj["ss_group_login"].ToString()))
                {
                    ss_group_login = EncryptionLibrary.DecryptText(JObj["ss_group_login"].ToString());
                }

                JObj["ss_group_login"] = ss_group_login;
            }
            if (JObj["ss_portfolio_id"] != null)
            {
                var ss_portfolio_id = JObj["ss_portfolio_id"].ToString();
                if (isBase64(JObj["ss_portfolio_id"].ToString()))
                {
                    ss_portfolio_id = EncryptionLibrary.DecryptText(JObj["ss_portfolio_id"].ToString());
                }

                JObj["ss_portfolio_id"] = ss_portfolio_id;
            }
            if (JObj["ss_subportfolio_id"] != null)
            {
                var ss_portfolio_id = JObj["ss_subportfolio_id"].ToString();
                if (isBase64(JObj["ss_subportfolio_id"].ToString()))
                {
                    ss_portfolio_id = EncryptionLibrary.DecryptText(JObj["ss_subportfolio_id"].ToString());
                }

                JObj["ss_subportfolio_id"] = ss_portfolio_id;
            }
            if (JObj["subportfolio_id"] != null)
            {
                var ss_portfolio_id = JObj["subportfolio_id"].ToString();
                if (isBase64(JObj["subportfolio_id"].ToString()))
                {
                    ss_portfolio_id = EncryptionLibrary.DecryptText(JObj["subportfolio_id"].ToString());
                }

                JObj["subportfolio_id"] = ss_portfolio_id;
            }
            if (JObj["group_id"] != null)
            {
                var ss_portfolio_id = JObj["group_id"].ToString();
                if (isBase64(JObj["group_id"].ToString()))
                {
                    ss_portfolio_id = EncryptionLibrary.DecryptText(JObj["group_id"].ToString());
                }

                JObj["group_id"] = ss_portfolio_id;
            }
            if (JObj["group_id"] != null)
            {
                var ss_portfolio_id = JObj["group_id"].ToString();
                if (isBase64(JObj["group_id"].ToString()))
                {
                    ss_portfolio_id = EncryptionLibrary.DecryptText(JObj["group_id"].ToString());
                }

                JObj["group_id"] = ss_portfolio_id;
            }
            if (JObj["user_id"] != null)
            {
                var user_id = JObj["user_id"].ToString();
                if (isBase64(JObj["user_id"].ToString()))
                {
                    user_id = EncryptionLibrary.DecryptText(JObj["user_id"].ToString());
                }

                JObj["user_id"] = user_id;
            }
            if (JObj["marketing_id"] != null)
            {
                var marketing_id = JObj["marketing_id"].ToString();
                if (isBase64(JObj["marketing_id"].ToString()))
                {
                    marketing_id = EncryptionLibrary.DecryptText(JObj["marketing_id"].ToString());
                }

                JObj["marketing_id"] = marketing_id;
            }
            if (JObj["user_edit"] != null)
            {
                var user_edit = JObj["user_edit"].ToString();
                if (isBase64(JObj["user_edit"].ToString()))
                {
                    user_edit = EncryptionLibrary.DecryptText(JObj["user_edit"].ToString());
                }

                JObj["user_edit"] = user_edit;
            }
            if (JObj["user_entry"] != null)
            {
                var user_entry = JObj["user_entry"].ToString();
                if (isBase64(JObj["user_entry"].ToString()))
                {
                    user_entry = EncryptionLibrary.DecryptText(JObj["user_entry"].ToString());
                }

                JObj["user_entry"] = user_entry;
            }
            if (JObj["user_input"] != null)
            {
                var user_input = JObj["user_input"].ToString();
                if (isBase64(JObj["user_input"].ToString()))
                {
                    user_input = EncryptionLibrary.DecryptText(JObj["user_input"].ToString());
                }

                JObj["user_input"] = user_input;
            }
            if (JObj["user_delete"] != null)
            {
                var user_id = JObj["user_delete"].ToString();
                if (isBase64(JObj["user_delete"].ToString()))
                {
                    user_id = EncryptionLibrary.DecryptText(JObj["user_delete"].ToString());
                }

                JObj["user_delete"] = user_id;
            }
            if (JObj["user"] != null)
            {
                var user_id = JObj["user"].ToString();
                if (isBase64(JObj["user"].ToString()))
                {
                    user_id = EncryptionLibrary.DecryptText(JObj["user"].ToString());
                }

                JObj["user"] = user_id;
            }
            if (JObj["post_by"] != null)
            {
                var user_id = JObj["post_by"].ToString();
                if (isBase64(JObj["post_by"].ToString()))
                {
                    user_id = EncryptionLibrary.DecryptText(JObj["post_by"].ToString());
                }

                JObj["post_by"] = user_id;
            }
            if (JObj["cancel_by"] != null)
            {
                var user_id = JObj["cancel_by"].ToString();
                if (isBase64(JObj["cancel_by"].ToString()))
                {
                    user_id = EncryptionLibrary.DecryptText(JObj["cancel_by"].ToString());
                }

                JObj["cancel_by"] = user_id;
            }
            if (JObj["delete_by"] != null)
            {
                var user_id = JObj["delete_by"].ToString();
                if (isBase64(JObj["delete_by"].ToString()))
                {
                    user_id = EncryptionLibrary.DecryptText(JObj["delete_by"].ToString());
                }

                JObj["delete_by"] = user_id;
            }

            if (JObj["approved_by"] != null)
            {
                //var user_input = EncryptionLibrary.DecryptText(JObj["approved_by"].ToString()); // SymmCrypto.Decrypt()
                var user_input = JObj["approved_by"].ToString();
                if (isBase64(JObj["approved_by"].ToString()))
                {
                    user_input = EncryptionLibrary.DecryptText(JObj["approved_by"].ToString());
                }
                JObj["approved_by"] = user_input;
            }
            if (JObj["done_by"] != null)
            {
                var user_input = JObj["done_by"].ToString();
                if (isBase64(JObj["done_by"].ToString()))
                {
                    user_input = EncryptionLibrary.DecryptText(JObj["done_by"].ToString());
                }
                JObj["done_by"] = user_input;
            }
            if (JObj["assign_to"] != null)
            {
                var user_input = JObj["assign_to"].ToString();
                if (isBase64(JObj["assign_to"].ToString()))
                {
                    user_input = EncryptionLibrary.DecryptText(JObj["assign_to"].ToString());
                }
                JObj["assign_to"] = user_input;
            }
            if (JObj["tested_by"] != null)
            {
                var user_input = JObj["tested_by"].ToString();
                if (isBase64(JObj["tested_by"].ToString()))
                {
                    user_input = EncryptionLibrary.DecryptText(JObj["tested_by"].ToString());
                }
                JObj["tested_by"] = user_input;
            }
            if (JObj["subportfolio_id"] != null)
            {
                var subportfolio_id = JObj["subportfolio_id"].ToString();
                if (isBase64(JObj["subportfolio_id"].ToString()))
                {
                    subportfolio_id = EncryptionLibrary.DecryptText(JObj["subportfolio_id"].ToString());
                }

                JObj["subportfolio_id"] = subportfolio_id;
            }
            if (JObj["portfolio_id"] != null)
            {
                var portfolio_id = JObj["portfolio_id"].ToString();
                if (isBase64(JObj["portfolio_id"].ToString()))
                {
                    portfolio_id = EncryptionLibrary.DecryptText(JObj["portfolio_id"].ToString());
                }

                JObj["portfolio_id"] = portfolio_id;
            }

            if (JObj["paramwhere"] != null)
            {
                var stamp = JObj["paramwhere"].ToString();
                stamp = stamp.Replace("'", "");
                JObj["paramwhere"] = stamp;
            }

            //ParamView
            {
                if (JObj["ParamView"] != null)
                {
                    if (string.IsNullOrEmpty(JObj["ParamView"].ToString()))
                    {
                        return JObj;
                    }
                    string[] param = JObj["ParamView"].ToString().Split(',');
                    for (int x = 0; x < param.Length; x++)
                    {
                        string parValue = this.getString(param[x], "'", "'");//param[x].LastIndexOf("'") ? "" : "";
                        if (isBase64(parValue))
                        {
                            param[x] = string.Format("'{0}'", EncryptionLibrary.DecryptText(parValue));
                        }
                    }

                    JObj["ParamView"] = string.Join(",", param);
                }
            }

            return JObj;
        }
        public DynamicParameters SpParameters(JObject obj, List<ParamFunction> ListParam)
        {
            DynamicParameters sp = new DynamicParameters();
            try
            {
                //List<ParamFunction> ListParam = new List<ParamFunction>();
                //ListParam = this.getListParamType(fn_name);

                foreach (JProperty x in obj.Properties())
                {
                    var valData = x.Value;
                    //long result;
                    var validName = string.Format("p_{0}", x.Name);
                    var pName = x.Name.ToString().Trim();
                    //int result;
                    //DateTime temp;
                    if (pName.ToLower() == "_message_")
                    {
                        continue;
                    }
                    if (pName.ToLower() == "_method_")
                    {
                        continue;
                    }
                    if (pName.ToLower() == "_lineno_")
                    {
                        continue;
                    }
                    ParamFunction ParamData = ListParam.Where(m => m.parameter_name.ToLower() == validName.ToLower()).FirstOrDefault();
                    if (ParamData == null)
                    {
                        throw new Exception("Parameter function not valid.");
                    }

                    //if (valData.ToString().ToLower() == "null")
                    //{
                    //    sp.Add("@" + validName, null);
                    //}

                    if (ParamData.data_type.ToLower() == "character varying" || ParamData.data_type.ToLower() == "character" || ParamData.data_type.ToLower() == "char" || ParamData.data_type.ToLower() == "text")
                    {
                        if (valData.ToString().ToLower() == "null")
                        {
                            sp.Add("@" + validName, null, dbType: DbType.String);
                        }
                        else
                        {
                            sp.Add("@" + validName, valData.ToString(), dbType: DbType.String);
                        }

                    }
                    else if (ParamData.data_type.ToLower() == "integer")
                    {
                        if (valData != null)
                        {
                            if (valData.ToString().ToLower() == "null")
                            {
                                sp.Add("@" + validName, null, dbType: DbType.Int32);
                                //return;
                            }
                            else
                            {                                
                                valData = string.IsNullOrEmpty(valData.ToString()) ? 0 : valData;
                                var intValue = valData.ToString().Replace(",", "");
                                if (intValue.Contains("."))
                                {
                                    sp.Add("@" + validName, Convert.ToInt32(Convert.ToDecimal(intValue)), dbType: DbType.Int32);
                                }
                                else
                                {
                                    sp.Add("@" + validName, Convert.ToInt32(intValue), dbType: DbType.Int32);
                                }

                            }
                        }

                    }
                    else if (ParamData.data_type.ToLower() == "decimal")
                    {
                        if (valData.ToString().ToLower() == "null")
                        {
                            sp.Add("@" + validName, null, dbType: DbType.Decimal);
                            //return;
                        }
                        else
                        {                            
                            valData = string.IsNullOrEmpty(valData.ToString()) ? 0 : valData;
                            var intValue = valData.ToString().Replace(",", "");
                            sp.Add("@" + validName, Convert.ToDecimal(intValue), dbType: DbType.Decimal);
                        }

                    }
                    else if (ParamData.data_type.ToLower() == "numeric")
                    {
                        if (valData.ToString().ToLower() == "null")
                        {
                            sp.Add("@" + validName, null, dbType: DbType.Int32);
                            //return;
                        }
                        else
                        {                            
                            valData = string.IsNullOrEmpty(valData.ToString()) ? 0 : valData;
                            var intValue = valData.ToString().Replace(",", "");
                            sp.Add("@" + validName, Convert.ToDecimal(intValue), dbType: DbType.Decimal);
                        }

                    }
                    else if (ParamData.data_type.ToLower() == "bigint")
                    {
                        if (valData.ToString().ToLower() == "null")
                        {
                            sp.Add("@" + validName, null, dbType: DbType.Int64);
                            //return;
                        }
                        else
                        {
                            valData = string.IsNullOrEmpty(valData.ToString()) ? 0 : valData;
                            var intValue = valData.ToString().Replace(",", "");
                            sp.Add("@" + validName, Convert.ToInt64(intValue), dbType: DbType.Int64);
                        }

                    }
                    else if (ParamData.data_type.ToLower() == "boolean")
                    {
                        if (valData.ToString().ToLower() == "null")
                        {
                            sp.Add("@" + validName, null, dbType: DbType.Boolean);
                            //return;
                        }
                        else
                        {
                            sp.Add("@" + validName, Convert.ToBoolean(valData), dbType: DbType.Boolean);
                        }

                    }
                    else if (ParamData.data_type.ToLower() == "json")
                    {
                        if (valData.ToString().ToLower() == "null")
                        {
                            sp.Add("@" + validName, null, (DbType)NpgsqlDbType.Json);
                            //return;
                        }
                        else
                        {
                            sp.Add("@" + validName, valData, (DbType)NpgsqlDbType.Json);
                        }

                    }
                    else if (ParamData.data_type.ToLower() == "datetime" || ParamData.data_type.ToLower() == "timestamp without time zone" || ParamData.data_type.ToLower() == "date" || ParamData.data_type.ToLower() == "timestamp")
                    {
                        if (valData.ToString().ToLower() == "null")
                        {
                            sp.Add("@" + validName, null, dbType: DbType.DateTime);
                            //return;
                        }
                        else
                        {
                            if (ParamData.data_type.ToLower() == "date")
                            {
                                sp.Add("@" + validName, DateTime.Parse(valData.ToString()), dbType: DbType.Date);
                            }
                            else
                            {
                                sp.Add("@" + validName, DateTime.Parse(valData.ToString()), dbType: DbType.DateTime);
                            }

                        }

                    }
                    else if (ParamData.data_type.ToLower() == "xid")
                    {
                        if (valData.ToString().ToLower() == "null")
                        {
                            sp.Add("@" + validName, null, dbType: DbType.DateTime);
                            //return;
                        }
                        else
                        {
                            sp.Add("@" + validName, Convert.ToInt32(valData.ToString()), dbType: DbType.Int32);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return sp;
        }
        public DynamicParameters SpParameterPortIn(List<ParamFunction> ListParam, ParamPortIn DataPort)
        {
            DynamicParameters sp = new DynamicParameters();
            try
            {
                ListParam.ForEach(delegate (ParamFunction dt)
                {
                    foreach (JProperty x in DataPort.data_port.Properties())
                    {
                        var valData = x.Value;
                        //long result;
                        var validName = string.Format("p_{0}", x.Name);
                        var pName = x.Name.ToString().Trim();
                        if (dt.parameter_name == validName)
                        {
                            if (dt.data_type.ToLower() == "character varying" || dt.data_type.ToLower() == "character" || dt.data_type.ToLower() == "char" || dt.data_type.ToLower() == "text")
                            {
                                if (valData.ToString().ToLower() == "null")
                                {
                                    sp.Add(dt.parameter_name, null, dbType: DbType.String);
                                }
                                else
                                {
                                    sp.Add(dt.parameter_name, valData.ToString(), dbType: DbType.String);
                                }

                            }
                            else if (dt.data_type.ToLower() == "integer")
                            {
                                if (valData != null)
                                {
                                    if (valData.ToString().ToLower() == "null")
                                    {
                                        sp.Add(dt.parameter_name, null, dbType: DbType.Int32);
                                        //return;
                                    }
                                    else
                                    {
                                        valData = string.IsNullOrEmpty(valData.ToString()) ? 0 : valData;
                                        if (valData.ToString().Contains("."))
                                        {
                                            sp.Add(dt.parameter_name, Convert.ToInt32(Convert.ToDecimal(valData.ToString())), dbType: DbType.Int32);
                                        }
                                        else
                                        {
                                            sp.Add(dt.parameter_name, Convert.ToInt32(valData.ToString()), dbType: DbType.Int32);
                                        }

                                    }
                                }

                            }
                            else if (dt.data_type.ToLower() == "decimal")
                            {
                                if (valData.ToString().ToLower() == "null")
                                {
                                    sp.Add(dt.parameter_name, null, dbType: DbType.Int32);
                                    //return;
                                }
                                else
                                {
                                    valData = string.IsNullOrEmpty(valData.ToString()) ? 0 : valData;
                                    sp.Add(dt.parameter_name, Convert.ToDecimal(valData.ToString()), dbType: DbType.Int32);
                                }

                            }
                            else if (dt.data_type.ToLower() == "numeric")
                            {
                                if (valData.ToString().ToLower() == "null")
                                {
                                    sp.Add(dt.parameter_name, null, dbType: DbType.Int32);
                                    //return;
                                }
                                else
                                {
                                    valData = string.IsNullOrEmpty(valData.ToString()) ? 0 : valData;
                                    sp.Add(dt.parameter_name, Convert.ToDecimal(valData.ToString()), dbType: DbType.Int32);
                                }

                            }
                            else if (dt.data_type.ToLower() == "bigint")
                            {
                                if (valData.ToString().ToLower() == "null")
                                {
                                    sp.Add(dt.parameter_name, null, dbType: DbType.Int64);
                                    //return;
                                }
                                else
                                {
                                    valData = string.IsNullOrEmpty(valData.ToString()) ? 0 : valData;
                                    sp.Add(dt.parameter_name, Convert.ToInt64(valData.ToString()), dbType: DbType.Int64);
                                }

                            }
                            else if (dt.data_type.ToLower() == "boolean")
                            {
                                if (valData.ToString().ToLower() == "null")
                                {
                                    sp.Add(dt.parameter_name, null, dbType: DbType.Boolean);
                                    //return;
                                }
                                else
                                {
                                    sp.Add(dt.parameter_name, Convert.ToBoolean(valData), dbType: DbType.Boolean);
                                }

                            }
                            else if (dt.data_type.ToLower() == "json")
                            {
                                if (valData.ToString().ToLower() == "null")
                                {
                                    sp.Add(dt.parameter_name, null, (DbType)NpgsqlDbType.Json);
                                    //return;
                                }
                                else
                                {
                                    sp.Add(dt.parameter_name, valData, (DbType)NpgsqlDbType.Json);
                                }

                            }
                            else if (dt.data_type.ToLower() == "datetime" || dt.data_type.ToLower() == "timestamp without time zone" || dt.data_type.ToLower() == "date" || dt.data_type.ToLower() == "timestamp")
                            {
                                if (valData.ToString().ToLower() == "null")
                                {
                                    sp.Add(dt.parameter_name, null, dbType: DbType.DateTime);
                                    //return;
                                }
                                else
                                {
                                    valData = valData.ToString().Replace(";@","").Replace(":@","");
                                    if (dt.data_type.ToLower() == "date")
                                    {                                    
                                        sp.Add(dt.parameter_name, DateTime.Parse(valData.ToString()), dbType: DbType.Date);
                                    }
                                    else
                                    {
                                        sp.Add(dt.parameter_name, DateTime.Parse(valData.ToString()), dbType: DbType.DateTime);
                                    }

                                }

                            }
                            //sp.Add(dt.parameter_name,);
                        }
                    }
                    if (dt.parameter_name.Contains("p_user_input") || dt.parameter_name.Contains("p_user_edit"))
                    {
                        if (!sp.ParameterNames.Contains("p_user_input") || !sp.ParameterNames.Contains("p_user_edit"))
                        {
                            sp.Add(dt.parameter_name, DataPort.user_input);
                        }
                    }
                    if (dt.parameter_name.Contains("p_ss_portfolio_id") || dt.parameter_name.Contains("p_portfolio_id"))
                    {
                        if (!sp.ParameterNames.Contains("p_ss_portfolio_id") || !sp.ParameterNames.Contains("p_portfolio_id"))
                        {
                            sp.Add(dt.parameter_name, DataPort.ss_portfolio_id);
                        }
                    }
                    if (dt.parameter_name.Contains("p_ss_subportfolio_id") || dt.parameter_name.Contains("p_subportfolio_id"))
                    {
                        if (!sp.ParameterNames.Contains("p_ss_subportfolio_id") || !sp.ParameterNames.Contains("p_subportfolio_id"))
                        {
                            sp.Add(dt.parameter_name, DataPort.ss_subportfolio_id);
                        }
                    }

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return sp;
        }
        public string sha256_hash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
        public string DecryptString(string data)
        {
            string _result = data;
            if (isBase64(data))
            {
                _result = EncryptionLibrary.DecryptText(data);
            }
            return _result;
        }
        public JObject SetFieldList(List<FieldSource> fieldSources, int len = 0, string ParamWhere = "", string definedColumn = "", bool List = false)
        {
            JObject _result = new JObject();
            Dictionary<int, string> OBjSize = new Dictionary<int, string>();
            string sField = string.Empty;
            string sFieldQuery = string.Empty;
            string sSize = string.Empty;
            string sType = string.Empty;
            string dtSize = string.Empty;
            string fieldWhere = string.Empty;
            string[] dfColumn = definedColumn != null ? definedColumn.Split(",") : null;
            //dfColumn = List ? null : dfColumn;
            //string sWhere = string.Empty;
            int dtType = 0;
            len = len == 0 ? fieldSources.Count : len > fieldSources.Count ? fieldSources.Count : len;
            if (fieldSources.Count > 0)
            {
                int n = 1;
                fieldSources.ForEach(delegate (FieldSource data)
                {
                    if (n <= len)
                    {

                        sField += string.Format("{0},", data.column_name);

                        if (dfColumn != null && List)
                        {
                            if (List)
                            {
                                if (data.data_type.ToLower().Contains("timestamp") || data.data_type.ToLower() == "datetime")
                                {
                                    //sFieldQuery += string.Format("TO_CHAR({0}, 'DD/MM/YYYY HH24:MI') as {0},", data.column_name);
                                    //sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd/mm/yyyy') as {0},", data.column_name);
                                    sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd Mon yyyy') as {0},", data.column_name);

                                }
                                else if (data.data_type.ToLower() == "date")
                                {
                                    //sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd/mm/yyyy') as {0},", data.column_name);
                                    sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd Mon yyyy') as {0},", data.column_name);
                                }
                                else
                                {
                                    sFieldQuery += string.Format("{0},", data.column_name);
                                }
                            }

                            for (int x = 0; x < dfColumn.Length; x++)
                            {
                                if (dfColumn[x].ToLower() == (data.column_name.ToLower()))
                                {
                                    // field query *convert format date
                                    if (!List)
                                    {
                                        if (data.data_type.ToLower().Contains("timestamp") || data.data_type.ToLower() == "datetime")
                                        {
                                            if (dfColumn[x].Contains("AS"))
                                            {
                                                string[] stF = dfColumn[x].Split("AS");
                                                //sFieldQuery += string.Format("TO_CHAR({0}, 'DD/MM/YYYY HH24:MI') as {1},", stF[0], stF[1]);
                                                //sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd/mm/yyyy') as {1},", stF[0], stF[1]);
                                                sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd Mon yyyy') as {1},", stF[0], stF[1]);
                                            }
                                            else
                                            {
                                                //sFieldQuery += string.Format("TO_CHAR({0}, 'DD/MM/YYYY HH24:MI') as {0},", data.column_name);
                                                //sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd/mm/yyyy') as {0},", data.column_name);
                                                sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd Mon yyyy') as {0},", data.column_name);
                                            }

                                        }
                                        else if (data.data_type.ToLower() == "date")
                                        {
                                            if (dfColumn[x].Contains("AS"))
                                            {
                                                string[] stF = dfColumn[x].Split("AS");
                                                //sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd/mm/yyyy') as {1},", stF[0], stF[1]);
                                                sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd Mon yyyy') as {1},", stF[0], stF[1]);
                                            }
                                            else
                                            {
                                                //sFieldQuery += string.Format("TO_CHAR({0}, 'DD/MM/YYYY HH24:MI') as {0},", data.column_name);
                                                //sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd/mm/yyyy') as {0},", data.column_name);
                                                sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd Mon yyyy') as {0},", data.column_name);
                                            }
                                        }
                                        else
                                        {
                                            sFieldQuery += string.Format("{0},", dfColumn[x]); //data.column_name);
                                        }
                                    }


                                    //define size
                                    if (data.data_type.ToLower() == "character varying" || data.data_type.ToLower() == "character" || data.data_type.ToLower() == "char" || data.data_type.ToLower() == "text")
                                    {
                                        dtSize = data.max_length <= 20 ? "S" : data.max_length >= 21 && data.max_length <= 49 ? "M" : data.max_length >= 50 ? "L" : "S";
                                    }
                                    else
                                    {
                                        dtSize = "S";
                                    }

                                    OBjSize.Add(x, dtSize);

                                    //sSize += dtSize + ",";
                                    string Stype = data.data_type.ToLower().Contains("timestamp") || data.data_type.ToLower().Contains("date") ? "T" : "S";
                                    fieldWhere += string.Format("{0}:{1},", dfColumn[x], Stype);//;
                                }
                            }

                        }
                        else
                        {
                            if (data.data_type.ToLower().Contains("timestamp") || data.data_type.ToLower() == "datetime")
                            {
                                //sFieldQuery += string.Format("TO_CHAR({0}, 'DD/MM/YYYY HH24:MI') as {0},", data.column_name);
                                //sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd/mm/yyyy') as {0},", data.column_name);
                                sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd Mon yyyy') as {0},", data.column_name);

                            }
                            else if (data.data_type.ToLower() == "date")
                            {
                                //sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd/mm/yyyy') as {0},", data.column_name);
                                sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd Mon yyyy') as {0},", data.column_name);
                            }
                            else
                            {
                                sFieldQuery += string.Format("{0},", data.column_name);
                            }

                            //define size
                            if (data.data_type.ToLower() == "character varying" || data.data_type.ToLower() == "character" || data.data_type.ToLower() == "char" || data.data_type.ToLower() == "text")
                            {
                                dtSize = data.max_length <= 20 ? "S" : data.max_length >= 21 && data.max_length <= 49 ? "M" : data.max_length >= 50 ? "L" : "S";
                            }
                            else
                            {
                                dtSize = "S";
                            }


                            sSize += dtSize + ",";
                            string Stype = data.data_type.ToLower().Contains("timestamp") || data.data_type.ToLower().Contains("date") ? "T" : "S";
                            fieldWhere += string.Format("{0}:{1},", data.column_name, Stype);//;
                        }



                        if (data.precision != null && data.scale != null)
                        {
                            dtType = 1;
                        }
                        else if (data.data_type.ToLower().Contains("timestamp") || data.data_type.ToLower().Contains("date"))
                        {
                            dtType = 2;
                        }
                        else
                        {
                            dtType = 3;
                        }
                        sType += string.Format("{0},", dtType);

                    }
                    n++;
                });

                sField = !string.IsNullOrEmpty(sField) ? sField.Remove(sField.LastIndexOf(",")) : sField;
                sFieldQuery = !string.IsNullOrEmpty(sFieldQuery) ? sFieldQuery.Remove(sFieldQuery.LastIndexOf(",")) : sFieldQuery;
                sSize = !string.IsNullOrEmpty(sSize) ? sSize.Remove(sSize.LastIndexOf(",")) : sSize;
                sType = !string.IsNullOrEmpty(sType) ? sType.Remove(sType.LastIndexOf(",")) : sType;
                fieldWhere = !string.IsNullOrEmpty(fieldWhere) ? fieldWhere.Remove(fieldWhere.LastIndexOf(",")) : fieldWhere;
                //sWhere = !string.IsNullOrEmpty(sWhere) ? "( " + sWhere.Remove(sWhere.LastIndexOf("OR")) + " )" : sWhere;
            }
            var dtsize = definedColumn != null ? definedColumn.Split(",") : null;
            if (dtsize != null)
            {
                if (dtsize[0] == "no")
                {
                    OBjSize.Add(0, "S");
                }
            }
            sSize = string.Join(",", OBjSize.OrderBy(key => key.Key).ToList().Select(s => s.Value).ToArray());
            //if (dt)
            //OBjSize.OrderBy()
            _result.Add("FieldQuery", sFieldQuery);
            _result.Add("Field", sField);
            _result.Add("DefineSize", sSize);
            _result.Add("DefineDataType", sType);
            _result.Add("fieldWhere", fieldWhere);
            //_result.Add("sWhereLikeList", sWhere);
            return _result;
        }
        public JObject SetFieldListNew(List<FieldSource> fieldSources, int len = 0, string ParamWhere = "", string definedColumn = "", bool List = false)
        {
            JObject _result = new JObject();
            Dictionary<int, string> OBjSize = new Dictionary<int, string>();
            string sField = string.Empty;
            string sFieldQuery = string.Empty;
            string sSize = string.Empty;
            string sType = string.Empty;
            string dtSize = string.Empty;
            string fieldWhere = string.Empty;
            string[] dfColumn = definedColumn != null ? definedColumn.Split(",") : null;
            //dfColumn = List ? null : dfColumn;
            //string sWhere = string.Empty;
            int dtType = 0;
            len = len == 0 ? fieldSources.Count : len > fieldSources.Count ? fieldSources.Count : len;
            if (fieldSources.Count > 0)
            {
                int n = 1;
                fieldSources.ForEach(delegate (FieldSource data)
                {
                    if (n <= len)
                    {

                        sField += string.Format("{0},", data.column_name);

                        if (dfColumn != null && List)
                        {
                            if (List)
                            {
                                if (data.data_type.ToLower().Contains("timestamp") || data.data_type.ToLower() == "datetime")
                                {
                                    //sFieldQuery += string.Format("TO_CHAR({0}, 'DD/MM/YYYY HH24:MI') as {0},", data.column_name);
                                    //sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd/mm/yyyy') as {0},", data.column_name);
                                    sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd Mon yyyy') as {0},", data.column_name);

                                }
                                else if (data.data_type.ToLower() == "date")
                                {
                                    //sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd/mm/yyyy') as {0},", data.column_name);
                                    sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd Mon yyyy') as {0},", data.column_name);
                                }
                                else
                                {
                                    sFieldQuery += string.Format("{0},", data.column_name);
                                }
                            }

                            for (int x = 0; x < dfColumn.Length; x++)
                            {
                                if (dfColumn[x].ToLower() == (data.column_name.ToLower()))
                                {
                                    // field query *convert format date
                                    if (!List)
                                    {
                                        if (data.data_type.ToLower().Contains("timestamp") || data.data_type.ToLower() == "datetime")
                                        {
                                            if (dfColumn[x].Contains("AS"))
                                            {
                                                string[] stF = dfColumn[x].Split("AS");
                                                //sFieldQuery += string.Format("TO_CHAR({0}, 'DD/MM/YYYY HH24:MI') as {1},", stF[0], stF[1]);
                                                //sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd/mm/yyyy') as {1},", stF[0], stF[1]);
                                                sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd Mon yyyy') as {1},", stF[0], stF[1]);
                                            }
                                            else
                                            {
                                                //sFieldQuery += string.Format("TO_CHAR({0}, 'DD/MM/YYYY HH24:MI') as {0},", data.column_name);
                                                //sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd/mm/yyyy') as {0},", data.column_name);
                                                sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd Mon yyyy') as {0},", data.column_name);
                                            }

                                        }
                                        else if (data.data_type.ToLower() == "date")
                                        {
                                            if (dfColumn[x].Contains("AS"))
                                            {
                                                string[] stF = dfColumn[x].Split("AS");
                                                //sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd/mm/yyyy') as {1},", stF[0], stF[1]);
                                                sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd Mon yyyy') as {1},", stF[0], stF[1]);
                                            }
                                            else
                                            {
                                                //sFieldQuery += string.Format("TO_CHAR({0}, 'DD/MM/YYYY HH24:MI') as {0},", data.column_name);
                                                //sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd/mm/yyyy') as {0},", data.column_name);
                                                sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd Mon yyyy') as {0},", data.column_name);
                                            }
                                        }
                                        else
                                        {
                                            sFieldQuery += string.Format("{0},", dfColumn[x]); //data.column_name);
                                        }
                                    }

                                    //sSize += dtSize + ",";
                                    string Stype = data.data_type.ToLower().Contains("timestamp") || data.data_type.ToLower().Contains("date") ? "T" : "S";
                                    fieldWhere += string.Format("{0}:{1},", dfColumn[x], Stype);//;
                                }
                            }

                        }
                        else
                        {
                            if (data.data_type.ToLower().Contains("timestamp") || data.data_type.ToLower() == "datetime")
                            {
                                //sFieldQuery += string.Format("TO_CHAR({0}, 'DD/MM/YYYY HH24:MI') as {0},", data.column_name);
                                //sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd/mm/yyyy') as {0},", data.column_name);
                                sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd Mon yyyy') as {0},", data.column_name);

                            }
                            else if (data.data_type.ToLower() == "date")
                            {
                                //sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd/mm/yyyy') as {0},", data.column_name);
                                sFieldQuery += string.Format("TO_CHAR({0} :: DATE, 'dd Mon yyyy') as {0},", data.column_name);
                            }
                            else
                            {
                                sFieldQuery += string.Format("{0},", data.column_name);
                            }

                            //define size
                            if (data.data_type.ToLower() == "character varying" || data.data_type.ToLower() == "character" || data.data_type.ToLower() == "char" || data.data_type.ToLower() == "text")
                            {
                                dtSize = data.max_length <= 20 ? "S" : data.max_length >= 21 && data.max_length <= 49 ? "M" : data.max_length >= 50 ? "L" : "S";
                            }
                            else
                            {
                                dtSize = "S";
                            }


                            sSize += dtSize + ",";
                            string Stype = data.data_type.ToLower().Contains("timestamp") || data.data_type.ToLower().Contains("date") ? "T" : "S";
                            fieldWhere += string.Format("{0}:{1},", data.column_name, Stype);//;
                        }



                        if (data.precision != null && data.scale != null)
                        {
                            dtType = 1;
                        }
                        else if (data.data_type.ToLower().Contains("timestamp") || data.data_type.ToLower().Contains("date"))
                        {
                            dtType = 2;
                        }
                        else
                        {
                            dtType = 3;
                        }
                        sType += string.Format("{0},", dtType);

                    }
                    n++;
                });

                ////define size
                if (List && dfColumn != null)
                {
                    for (int x = 0; x < dfColumn.Length; x++)
                    {
                        if (dfColumn[x] == "no") continue;
                        var dataField = fieldSources.Where(w => w.column_name.ToLower() == dfColumn[x].ToLower()).FirstOrDefault();
                        if (dataField == null) continue;
                        //if (dataField != null)
                        //{
                        if (dataField.data_type.ToLower() == "character varying" || dataField.data_type.ToLower() == "character" || dataField.data_type.ToLower() == "char" || dataField.data_type.ToLower() == "text")
                        {
                            dtSize = dataField.max_length <= 20 ? "S" : dataField.max_length >= 21 && dataField.max_length <= 49 ? "M" : dataField.max_length >= 50 ? "L" : "S";
                        }
                        else
                        {
                            dtSize = "S";
                        }

                        OBjSize.Add(x, dtSize);
                        //}
                    }

                }


                sField = !string.IsNullOrEmpty(sField) ? sField.Remove(sField.LastIndexOf(",")) : sField;
                sFieldQuery = !string.IsNullOrEmpty(sFieldQuery) ? sFieldQuery.Remove(sFieldQuery.LastIndexOf(",")) : sFieldQuery;
                sSize = !string.IsNullOrEmpty(sSize) ? sSize.Remove(sSize.LastIndexOf(",")) : sSize;
                sType = !string.IsNullOrEmpty(sType) ? sType.Remove(sType.LastIndexOf(",")) : sType;
                fieldWhere = !string.IsNullOrEmpty(fieldWhere) ? fieldWhere.Remove(fieldWhere.LastIndexOf(",")) : fieldWhere;
                //sWhere = !string.IsNullOrEmpty(sWhere) ? "( " + sWhere.Remove(sWhere.LastIndexOf("OR")) + " )" : sWhere;
            }
            var dtsize = definedColumn != null ? definedColumn.Split(",") : null;
            if (dtsize != null)
            {
                if (dtsize[0] == "no")
                {
                    OBjSize.Add(0, "S");
                }
            }
            sSize = string.Join(",", OBjSize.OrderBy(key => key.Key).ToList().Select(s => s.Value).ToArray());
            //if (dt)
            //OBjSize.OrderBy()
            _result.Add("FieldQuery", sFieldQuery);
            _result.Add("Field", sField);
            _result.Add("DefineSize", sSize);
            _result.Add("DefineDataType", sType);
            _result.Add("fieldWhere", fieldWhere);
            //_result.Add("sWhereLikeList", sWhere);
            return _result;
        }
        public string FormatSort(string SortString)
        {
            string _result = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(SortString)) return _result;

                string[] sSort = SortString.Split(",");
                for (int x = 0; x < sSort.Length; x++)
                {
                    string[] sField = sSort[x].Split(" ");
                    sField = sField.Where(w => w.Trim() != "").ToArray();
                    string field = sField[0].ToString().Trim();
                    string value = sField.Count() > 1 ? sField[1].ToString().Trim() : "ASC";

                    //_result += string.Format(" \"{0}\" {1} ,", field, value);
                    _result += string.Format(" {0} {1} ,", field, value);
                }
                _result = !string.IsNullOrEmpty(_result) ? _result.Remove(_result.LastIndexOf(",")) : _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public string FormatField(string SortString)
        {
            string _result = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(SortString)) return _result;
                string[] sSort = SortString.Trim().Split(",");
                for (int x = 0; x < sSort.Length; x++)
                {
                    _result += string.Format(" {0},", sSort[x]);
                }
                _result = !string.IsNullOrEmpty(_result) ? _result.Remove(_result.LastIndexOf(",")) : _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public string FormatParamView(string ParamView)
        {
            string _result = string.Empty;
            try
            {
                //_result = EncryptionLibrary.DecryptText(data);
                string[] sParam = ParamView.Split(",");
                for (int x = 0; x < sParam.Length; x++)
                {
                    string param = sParam[x].Replace("'", "");

                    sParam[x] = isBase64(param) ? string.Format("'{0}'", EncryptionLibrary.DecryptText(param)) : string.Format("'{0}'", param);

                }
                _result = string.Join(",", sParam);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public JObject SetParamWithValueOutput(JObject obj, JObject sParamOutput)
        {
            try
            {
                foreach (JProperty x in obj.Properties())
                {
                    var val = x.Value.ToString();
                    var Name = x.Name.ToString().Trim();
                    if (val.ToLower().Contains("_output"))
                    {
                        string ParamName = val.Replace("_OUTPUT", "").Replace("_output", "");
                        string[] Params = ParamName.Split(".");
                        string VarParamName = Params[0];
                        string FieldParam = Params[1];

                        if (sParamOutput[VarParamName] != null)
                        {
                            var dataVarParam = sParamOutput[VarParamName];
                            obj[Name] = dataVarParam[FieldParam];
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return obj;

        }

        public List<JObject> ObjectToLower(List<dynamic> list/*, string SpName*/)
        {
            List<JObject> Ljob = new List<JObject>();
            try
            {
                if (list.Count != 0)
                {
                    //List<GetPropertySource> dt = this.GetPropertyResultSP(SpName);
                    foreach (var x in list.ToList())
                    {
                        var item = x as IDictionary<string, Object>;
                        var itemKey = item.Keys;
                        int i = 0;
                        JObject JOb = new JObject();


                        object[] values = new object[itemKey.Count];

                        foreach (var info in itemKey)
                        {
                            if (item[info] != null)
                            {
                                var val = item[info].ToString();
                                int result;
                                double db;
                                DateTime temp;
                                if (int.TryParse(val, out result))
                                {
                                    if (val.Substring(0, 1) == "0")
                                    {
                                        JOb.Add(info.ToLower(), val);
                                    }
                                    else
                                    {
                                        JOb.Add(info.ToLower(), result);
                                    }
                                }
                                else if (DateTime.TryParse(val, out temp))
                                {
                                    //otnay
                                    if (val.Length < 10)
                                    { JOb.Add(info.ToLower(), val); }
                                    else
                                    { JOb.Add(info.ToLower(), temp); }

                                }
                                else if (double.TryParse(val, out db))
                                {
                                    if (val.Substring(0, 1) == "0")
                                    {
                                        JOb.Add(info.ToLower(), val);
                                    }
                                    else
                                    {
                                        //JOb.Add(info.ToLower(), db);
                                        JOb.Add(info.ToLower(), item[info].ToString());
                                    }

                                }
                                else
                                {
                                    JOb.Add(info.ToLower(), item[info].ToString());
                                }

                                //}


                            }
                            else
                            {
                                JOb.Add(info.ToLower(), null);
                            }
                            i++;
                        }

                        Ljob.Add(JOb);


                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return Ljob;
        }

        public string DecryptDataString(string data)
        {
            string _result = string.Empty;
            string valStrEncrypt = string.Empty;
            string valStrDecrypt = string.Empty;

            if (data.ToLower().Contains("user"))
            {
                if (data.ToLower().Contains("user_edit"))
                {
                    valStrEncrypt = this.getString(data, "user_edit='", "'");
                    valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "user_edit='", "'") : valStrEncrypt;
                    if (isBase64(valStrEncrypt))
                    {
                        valStrDecrypt = EncryptionLibrary.DecryptText(valStrEncrypt);
                    }

                    if (!string.IsNullOrEmpty(valStrDecrypt))
                        data = data.Replace(valStrEncrypt, valStrDecrypt);

                    valStrDecrypt = string.Empty;

                }

                if (data.ToLower().Contains("user_input"))
                {
                    valStrEncrypt = this.getString(data, "user_input='", "'");
                    valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "user_input='", "'") : valStrEncrypt;
                    if (isBase64(valStrEncrypt))
                    {
                        valStrDecrypt = EncryptionLibrary.DecryptText(valStrEncrypt);
                    }

                    if (!string.IsNullOrEmpty(valStrDecrypt))
                        data = data.Replace(valStrEncrypt, valStrDecrypt);

                    valStrDecrypt = string.Empty;
                }
                //

                if (data.ToLower().Contains("user_id_login"))
                {
                    valStrEncrypt = this.getString(data, "user_id_login='", "'");
                    valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "user_id_login='", "'") : valStrEncrypt;
                    if (isBase64(valStrEncrypt))
                    {
                        valStrDecrypt = EncryptionLibrary.DecryptText(valStrEncrypt);
                    }

                    if (!string.IsNullOrEmpty(valStrDecrypt))
                        data = data.Replace(valStrEncrypt, valStrDecrypt);

                    valStrDecrypt = string.Empty;
                }

                if (data.ToLower().Contains("user_id"))
                {
                    valStrEncrypt = this.getString(data, "user_id='", "'");
                    valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "user_id<>'", "'") : valStrEncrypt;
                    valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "user_id!='", "'") : valStrEncrypt;
                    
                    if (isBase64(valStrEncrypt))
                    {
                        valStrDecrypt = EncryptionLibrary.DecryptText(valStrEncrypt);
                    }

                    if (!string.IsNullOrEmpty(valStrDecrypt))
                        data = data.Replace(valStrEncrypt, valStrDecrypt);

                    valStrDecrypt = string.Empty;
                }

                if (data.ToLower().Contains("user"))
                {
                    valStrEncrypt = this.getString(data, "user='", "'");
                    valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "user<>'", "'") : valStrEncrypt;
                    valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "user!='", "'") : valStrEncrypt;
                    if (isBase64(valStrEncrypt))
                    {
                        valStrDecrypt = EncryptionLibrary.DecryptText(valStrEncrypt);
                    }

                    if (!string.IsNullOrEmpty(valStrDecrypt))
                        data = data.Replace(valStrEncrypt, valStrDecrypt);

                    valStrDecrypt = string.Empty;
                }
            }

            if (data.ToLower().Contains("ss_group_id"))
            {
                valStrEncrypt = this.getString(data, "ss_group_id='", "'");
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "ss_group_id='", "'") : valStrEncrypt;
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "ss_group_id<>'", "'") : valStrEncrypt;
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "ss_group_id!='", "'") : valStrEncrypt;
                if (isBase64(valStrEncrypt))
                {
                    valStrDecrypt = EncryptionLibrary.DecryptText(valStrEncrypt);
                }

                if (!string.IsNullOrEmpty(valStrDecrypt))
                    data = data.Replace(valStrEncrypt, valStrDecrypt);

                valStrDecrypt = string.Empty;
            }

            if (data.ToLower().Contains("group_id"))
            {
                valStrEncrypt = this.getString(data, "group_id='", "'");
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "group_id='", "'") : valStrEncrypt;
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "group_id<>'", "'") : valStrEncrypt;
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "group_id!='", "'") : valStrEncrypt;
                if (isBase64(valStrEncrypt))
                {
                    valStrDecrypt = EncryptionLibrary.DecryptText(valStrEncrypt);
                }

                if (!string.IsNullOrEmpty(valStrDecrypt))
                    data = data.Replace(valStrEncrypt, valStrDecrypt);

                valStrDecrypt = string.Empty;
            }

            if (data.ToLower().Contains("ss_portfolio_id"))
            {
                valStrEncrypt = this.getString(data, "ss_portfolio_id='", "'");
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "ss_portfolio_id='", "'") : valStrEncrypt;
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "ss_portfolio_id<>'", "'") : valStrEncrypt;
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "ss_portfolio_id!='", "'") : valStrEncrypt;
                if (isBase64(valStrEncrypt))
                {
                    valStrDecrypt = EncryptionLibrary.DecryptText(valStrEncrypt);
                }

                if (!string.IsNullOrEmpty(valStrDecrypt))
                    data = data.Replace(valStrEncrypt, valStrDecrypt);

                valStrDecrypt = string.Empty;
            }
            if (data.ToLower().Contains("portfolio_id"))
            {
                valStrEncrypt = this.getString(data, "portfolio_id='", "'");
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "portfolio_id='", "'") : valStrEncrypt;
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "portfolio_id<>'", "'") : valStrEncrypt;
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "portfolio_id!='", "'") : valStrEncrypt;
                if (isBase64(valStrEncrypt))
                {
                    valStrDecrypt = EncryptionLibrary.DecryptText(valStrEncrypt);
                }

                if (!string.IsNullOrEmpty(valStrDecrypt))
                    data = data.Replace(valStrEncrypt, valStrDecrypt);

                valStrDecrypt = string.Empty;
            }

            if (data.ToLower().Contains("ss_subportfolio_id"))
            {
                valStrEncrypt = this.getString(data, "ss_subportfolio_id='", "'");
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "ss_subportfolio_id='", "'") : valStrEncrypt;
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "ss_subportfolio_id<>'", "'") : valStrEncrypt;
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "ss_subportfolio_id!='", "'") : valStrEncrypt;
                if (isBase64(valStrEncrypt))
                {
                    valStrDecrypt = EncryptionLibrary.DecryptText(valStrEncrypt);
                }

                if (!string.IsNullOrEmpty(valStrDecrypt))
                    data = data.Replace(valStrEncrypt, valStrDecrypt);

                valStrDecrypt = string.Empty;
            }


            if (data.ToLower().Contains("subportfolio_id"))
            {
                valStrEncrypt = this.getString(data, "subportfolio_id='", "'");
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "subportfolio_id='", "'") : valStrEncrypt;
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "subportfolio_id<>'", "'") : valStrEncrypt;
                valStrEncrypt = string.IsNullOrEmpty(valStrEncrypt) ? this.getString(data, "subportfolio_id!='", "'") : valStrEncrypt;
                if (isBase64(valStrEncrypt))
                {
                    valStrDecrypt = EncryptionLibrary.DecryptText(valStrEncrypt);
                }

                if (!string.IsNullOrEmpty(valStrDecrypt))
                    data = data.Replace(valStrEncrypt, valStrDecrypt);

                valStrDecrypt = string.Empty;
            }
            _result = data;
            return _result;
        }
        public string sWhereLikeList(string sField, string sParam)
        {
            string _result = string.Empty;
            try
            {
                string[] fields = sField.Split(',');
                for (int x = 0; x < fields.Count(); x++)
                {
                    string[] sFIeld = fields[x].Split(':');
                    if (sFIeld[0].ToString().ToLower() == "no")
                    {
                        continue;
                    }

                    sFIeld[0] = string.Format("{0}", sFIeld[0]);
                    if (sFIeld.Count() > 1)
                    {
                        if (sFIeld[1] == "T")
                        {
                            _result += string.Format("lower(TO_CHAR({0}, 'DD/MM/YYYY HH24:MI')) LIKE '%{1}%' OR ", sFIeld[0], sParam.ToLower());
                        }
                        else
                        {
                            _result += string.Format("lower({0}::varchar) LIKE '%{1}%' OR ", sFIeld[0], sParam.ToLower());
                        }
                    }
                    else
                    {
                        _result += string.Format("lower({0}::varchar) LIKE '%{1}%' OR ", sFIeld[0], sParam.ToLower());
                    }


                }
                _result = !string.IsNullOrEmpty(_result) ? "( " + _result.Remove(_result.LastIndexOf("OR")) + " )" : _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public string SparamViewDecrypt(string ParamView)
        {
            string _result = string.Empty;
            try
            {
                string[] fields = ParamView.Split(',');
                for (int x = 0; x < fields.Count(); x++)
                {
                    if (fields[x] != "")
                    {
                        string Key = fields[x].Replace("'", "");

                        if (isBase64(Key))
                        {
                            fields[x] = string.Format("'{0}'", EncryptionLibrary.DecryptText(Key));
                        }
                    }

                }

                _result = string.Join(",", fields);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public string SetFieldListLookup(string SourceField, string DisplayField)
        {
            string _result = string.Empty;
            try
            {
                string[] FieldLookups = SourceField.Split(',');
                string[] Displays = DisplayField.Split(',');
                string DisplayLookup = string.Empty;
                DisplayLookup = "concat(";
                for (int x = 0; x < Displays.Length; x++)
                {
                    DisplayLookup += string.Format("{0}::varchar,' | ',", Displays[x]);
                }
                DisplayLookup = !string.IsNullOrEmpty(DisplayLookup) ? DisplayLookup.Remove(DisplayLookup.LastIndexOf(",' | ',")) : DisplayLookup;
                DisplayLookup += ")";

                for (int i = 0; i < FieldLookups.Length; i++)
                {
                    if (i == 0)
                    {
                        _result += string.Format("{0} as id,", FieldLookups[i]);
                    }
                    else if (i == 1)
                    {
                        _result += string.Format("{0} as label,", DisplayLookup);
                        _result += string.Format("{0},", FieldLookups[i]);
                    }
                    else
                    {
                        _result += string.Format("{0},", FieldLookups[i]);
                    }
                }
                _result = !string.IsNullOrEmpty(_result) ? _result.Remove(_result.LastIndexOf(",")) : _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public bool isBase64(string Base64)
        {
            bool v;
            try
            {
                EncryptionLibrary.DecryptText(Base64);
                v = true;
            }
            catch (Exception ex)
            {
                v = false;
            }
            return v;
        }
        public string getString(string strSource, string strStart, string strEnd)
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
    }
}
