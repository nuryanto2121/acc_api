using Acc.Api.DataAccess;
using Acc.Api.Enum;
using Acc.Api.Helper;
using Acc.Api.Interface;
using Acc.Api.Models;
using Dapper;
using EncryptLibrary.AES256Encryption;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class DynamicService : IDynamicService
    {
        IConfiguration config;
        private string connectionString;
        private OptionTemplateRepo OptionRepo;
        public string MvSpName = string.Empty;
        private FunctionString fn;
        Output _result = new Output();

        public DynamicService(IConfiguration Configuration)
        {
            config = Configuration;
            connectionString = Tools.ConnectionString(Configuration);
            OptionRepo = new OptionTemplateRepo(Configuration);
            fn = new FunctionString(connectionString);
        }
        public Output execute(int? id, int lastupdatestamp, int? line_no, string option_url, bool isDelete = false)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    if (id == 0 || id == null)
                    {
                        throw new Exception("Parameter id not valid.");
                    }
                    if (line_no == null)
                    {
                        throw new Exception("Parameter line_no not valid.");
                    }
                    if (string.IsNullOrEmpty(option_url))
                    {
                        throw new Exception("Parameter option_url not valid.");
                    }
                    DynamicParameters spParam = new DynamicParameters();

                    conn.Open();
                    string method = string.Empty;
                    if (isDelete)
                    {
                        method = SQL.Method.Aggregate.Delete.ToString().ToUpper();
                    }
                    else
                    {
                        method = SQL.Method.Aggregate.GetById.ToString().ToUpper();
                    }
                    MvSpName = (from OD in OptionRepo.GetListUrl(option_url).Where(x => x.method_api.ToUpper() == method && x.line_no == line_no)
                                select OD.sp).FirstOrDefault();
                    if (string.IsNullOrEmpty(MvSpName))
                    {
                        throw new Exception("Please Contact Your Administrator (Table setting null or Empty).");
                    }
                    spParam.Add("@p_row_id", id, dbType: DbType.Int32);
                    spParam.Add("@p_lastupdatestamp", lastupdatestamp, dbType: DbType.Int32);

                    var datas = conn.Query(MvSpName, spParam, commandType: CommandType.StoredProcedure);
                    _result.Data = datas;
                }
                catch (Exception ex)
                {
                    throw ex;
                    //_result = Tools.Error(ex);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }

            }
            return _result;
        }

        public Output execute(JObject JModel, SQL.Method.Aggregate Method)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    if (JModel.Count == 0)
                    {
                        throw new Exception("Data not valid.");
                    }
                    conn.Open();
                    string option_url = string.Empty;
                    int line_no;

                    DynamicParameters spParam = new DynamicParameters();
                    if (JModel["option_url_v2"] != null)
                    { option_url = JModel["option_url_v2"].ToString(); }
                    else
                    {
                        if (JModel["option_url"] == null)
                        {
                            throw new Exception("invalid parameter option_url.");
                        }
                        option_url = JModel["option_url"].ToString();
                    }

                    if (JModel["line_no_v2"] != null)
                    { line_no = Convert.ToInt32(JModel["line_no_v2"].ToString()); }
                    else
                    {
                        if (JModel["line_no"] == null)
                        {
                            throw new Exception("invalid parameter line_no.");
                        }

                        line_no = Convert.ToInt32(JModel["line_no"].ToString());
                    }

                    MvSpName = (from OD in OptionRepo.GetListUrl(option_url).Where(x => x.method_api.ToUpper() == Method.ToString().ToUpper() && x.line_no == line_no)
                                select OD.sp).FirstOrDefault();

                    if (string.IsNullOrEmpty(MvSpName))
                    {
                        throw new Exception("Please Contact Your Administrator (Table setting null or Empty).");
                    }
                    if (JModel["option_url_v2"] != null)
                    { JModel.Remove("option_url_v2"); }
                    else { JModel.Remove("option_url"); }

                    if (JModel["line_no_v2"] != null)
                    { JModel.Remove("line_no_v2"); }
                    else { JModel.Remove("line_no"); }


                    JModel = fn.DecryptData(JModel);
                    spParam = fn.SpParameters(JModel, OptionRepo.getListParamType(MvSpName));

                    //MvSpName = MvSpName + "";
                    var datas = conn.Query(MvSpName, spParam, commandType: CommandType.StoredProcedure);
                    _result.Data = datas;

                    if (Method == SQL.Method.Aggregate.Delete || Method == SQL.Method.Aggregate.Update)
                    {
                        _result.Message = "Data Has Been " + Method.ToString() + "d Successfully";
                    }
                    else if (Method == SQL.Method.Aggregate.Save)
                    {
                        _result.Message = "Data Has Been Inserted Successfully";
                    }
                    else if (Method == SQL.Method.Aggregate.GetById)
                    {
                        _result.Data = fn.ObjectToLower(datas.ToList());
                    }


                }
                catch (Exception ex)
                {
                    _result = Tools.Error(ex);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }

            return _result;
        }

        public DTResultListDyn<dynamic> executeList(ParamList JModel, SQL.Method.Aggregate Method)
        {
            var OutputList = new DTResultListDyn<dynamic>();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    //if (JModel.Count == 0)
                    //{
                    //    throw new Exception("Data no valid.");
                    //}
                    string option_url = JModel.option_url;
                    int line_no = JModel.line_no;
                    string sWhere = string.Empty;
                    string user_id = fn.DecryptString(JModel.user_id);
                    string subportfolio_id = fn.DecryptString(JModel.subportfolio_id);
                    int JmlahField = config.GetValue<int>("appSetting:JumlahFieldList");
                    int iStart = JModel.current_page;
                    int iPerPage = JModel.per_page;
                    string sSortField = string.IsNullOrEmpty(JModel.sort_field) ? "ORDER BY time_edit desc" : "ORDER BY " + fn.FormatSort(JModel.sort_field);
                    bool isViewFUnction = string.IsNullOrEmpty(JModel.param_view) ? false : true;
                    DynamicParameters spParam = new DynamicParameters();



                    MvSpName = (from OD in OptionRepo.GetListUrl(option_url).Where(x => x.method_api.ToUpper() == Method.ToString().ToUpper() && x.line_no == line_no)
                                select OD.sp).FirstOrDefault();

                    if (string.IsNullOrEmpty(MvSpName))
                    {
                        throw new Exception("Please Contact Your Administrator (Table setting null or Empty).");
                    }
                    var dataDefineColumn = OptionRepo.GetDefineColumn(user_id, subportfolio_id, option_url, line_no);

                    var fieldsource = OptionRepo.getListFieldType(MvSpName, isViewFUnction);
                    JObject dataFieldList = fn.SetFieldList(fieldsource, JmlahField, definedColumn: dataDefineColumn?.column_field, List: true);
                    string AllColumn = fn.SetFieldList(fieldsource, fieldsource.Count)["Field"].ToString();
                    string allCoulumnQUery = dataFieldList["FieldQuery"].ToString();
                    string DefineSize = dataFieldList["DefineSize"].ToString();
                    string fieldWhere = dataFieldList["fieldWhere"].ToString();

                    #region Field
                    string DefineColumn = string.Empty;
                    string DefineColumnFormat = string.Empty;
                    //cek defineColumn dlu

                    if (dataDefineColumn != null)
                    {

                        DefineColumn = dataDefineColumn.column_field;
                        DefineColumnFormat = fn.FormatField(dataDefineColumn.column_field);

                    }
                    else
                    {
                        DefineColumn = "no," + dataFieldList["Field"].ToString();
                        DefineColumnFormat = fn.FormatField(dataFieldList["Field"].ToString());
                        OptionRepo.InsertDefineColumn(user_id, subportfolio_id, option_url, line_no, DefineColumn);
                    }
                    #endregion

                    #region Parameter Where
                    var ParamWHere = JModel.param_where;
                    var initialwhere = JModel.initial_where;
                    initialwhere = string.IsNullOrEmpty(initialwhere) ? initialwhere : "WHERE " + initialwhere;
                    sWhere = fn.DecryptDataString(initialwhere);

                    string sWhereLike = fn.sWhereLikeList(fieldWhere, ParamWHere); //fn.sWhereLikeList(DefineColumn, ParamWHere)
                    sWhere += !string.IsNullOrEmpty(ParamWHere) ? !string.IsNullOrEmpty(sWhere) ? " AND " + sWhereLike : sWhereLike : string.Empty;
                    #endregion

                    //conn.Open();
                    if (!string.IsNullOrEmpty(JModel.param_view))
                    {
                        MvSpName = string.IsNullOrEmpty(JModel.param_view) ? MvSpName : string.Format("{0}({1})", MvSpName, fn.FormatParamView(JModel.param_view));
                    }
                    OutputList.Data = this.QueryList(MvSpName, iStart, iPerPage, sSortField, sWhere, allCoulumnQUery); //allCoulumnQUery,DefineColumnFormat
                    OutputList.Total = Convert.ToInt32(this.CountList(MvSpName, sWhere));
                    OutputList.Current_Page = iStart;
                    OutputList.Last_Page = Convert.ToInt32(Math.Ceiling((Convert.ToDecimal(OutputList.Total) / iPerPage)));

                    sWhere = string.IsNullOrEmpty(sWhere) ? "X" : sWhere;
                    var Encript = string.Join(":", new string[] {
                        //option_url.ToString(),
                        //line_no.ToString(),
                        MvSpName,
                        sWhere,
                        DefineColumn,
                        sSortField

                    });

                    OutputList.AllColumn = AllColumn;
                    OutputList.ExportToken = EncryptionLibrary.EncryptText(Encript);
                    OutputList.DefineColumn = DefineColumn;
                    OutputList.DefineSize = DefineSize;
                }
                catch (Exception ex)
                {
                    //throw ex;

                    OutputList.Error = true;// Tools.Error(ex);
                    OutputList.Message = ex.Message;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return OutputList;
        }

        public Output executeDataList(JObject JModel, SQL.Method.Aggregate Method)
        {
            var OutputList = new Output();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    if (JModel.Count == 0)
                    {
                        throw new Exception("Data no valid.");
                    }
                    string option_url = string.Empty;
                    int line_no;
                    string sWhere = string.Empty;
                    string user_id = fn.DecryptString(JModel["user_id"].ToString());
                    string subportfolio_id = JModel["subportfolio_id"].ToString();
                    int JmlahField = config.GetValue<int>("appSetting:JumlahFieldList");
                    int iStart = Convert.ToInt32(JModel["CurrentPage"]);
                    int iPerPage = Convert.ToInt32(JModel["PerPage"]);
                    string sSortField = string.IsNullOrEmpty(JModel["SortField"].ToString()) ? string.Empty : "ORDER BY " + fn.FormatSort(JModel["SortField"].ToString());

                    DynamicParameters spParam = new DynamicParameters();
                    if (JModel["option_url_v2"] != null)
                    { option_url = JModel["option_url_v2"].ToString(); }
                    else
                    {
                        if (JModel["option_url"] == null)
                        {
                            throw new Exception("invalid parameter option_url.");
                        }
                        option_url = JModel["option_url"].ToString();
                    }

                    if (JModel["line_no_v2"] != null)
                    { line_no = Convert.ToInt32(JModel["line_no_v2"].ToString()); }
                    else
                    {
                        if (JModel["line_no"] == null)
                        {
                            throw new Exception("invalid parameter line_no.");
                        }

                        line_no = Convert.ToInt32(JModel["line_no"].ToString());
                    }

                    MvSpName = (from OD in OptionRepo.GetListUrl(option_url).Where(x => x.method_api.ToUpper() == Method.ToString().ToUpper() && x.line_no == line_no)
                                select OD.sp).FirstOrDefault();
                    if (string.IsNullOrEmpty(MvSpName))
                    {
                        throw new Exception("Please Contact Your Administrator (Table setting null or Empty).");
                    }

                    var fieldsource = OptionRepo.getListFieldType(MvSpName);
                    if (fieldsource == null)
                    {
                        string msg = string.Format("Please Contact Your Administrator ({0}, can't read field).", MvSpName);
                        throw new Exception("");
                    }
                    JObject dataFieldList = fn.SetFieldList(fieldsource, fieldsource.Count);
                    string AllColumn = fn.SetFieldList(fieldsource, fieldsource.Count)["Field"].ToString();
                    string DefineSize = dataFieldList["DefineSize"].ToString();

                    #region Field
                    //string DefineColumn = AllColumn;// string.Empty;                  
                    #endregion

                    #region Parameter Where
                    //var ParamWHere = JModel["ParamWhere"].ToString();
                    var initialwhere = JModel["InitialWhere"].ToString();
                    initialwhere = string.IsNullOrEmpty(initialwhere) ? initialwhere : "WHERE " + initialwhere;
                    sWhere = fn.DecryptDataString(initialwhere);

                    //sWhere += !string.IsNullOrEmpty(ParamWHere) ? !string.IsNullOrEmpty(sWhere) ? " AND " + fn.sWhereLikeList(AllColumn, ParamWHere) : fn.sWhereLikeList(AllColumn, ParamWHere) : string.Empty;
                    #endregion

                    //conn.Open();
                    if (JModel["ParamView"] != null)
                    {
                        MvSpName = string.IsNullOrEmpty(JModel["ParamView"].ToString()) ? MvSpName : string.Format("{0}({1})", MvSpName, JModel["ParamView"].ToString());
                    }
                    //OutputList.Data = this.QueryList(MvSpName, iStart, iPerPage, sSortField, sWhere, AllColumn); //DefineColumnFormat
                    OutputList.Data = this.QueryList(MvSpName, sWhere);

                }
                catch (Exception ex)
                {
                    Tools.Error(ex);
                    //OutputList.Error = true;
                    //OutputList.Message = ex.Message;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return OutputList;
        }

        public Output executeMultiDelete(DynamicMultiParam JModel, SQL.Method.Aggregate Method)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {

                if (JModel.Data.Count == 0 || string.IsNullOrEmpty(JModel.option_url))
                {
                    throw new Exception("Data not Valid.");
                }

                List<JObject> LOutPut = new List<JObject>();

                MvSpName = (from OD in OptionRepo.GetListUrl(JModel.option_url).Where(x => x.method_api.ToUpper() == Method.ToString().ToUpper() && x.line_no == JModel.line_no)
                            select OD.sp).FirstOrDefault();

                if (string.IsNullOrEmpty(MvSpName))
                {
                    throw new Exception("Please Contact Your Administrator (Table setting null or Empty).");
                }

                string SpName = MvSpName;
                int n = 1;
                JModel.Data.ForEach(delegate (JObject data)
                {
                    if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                        conn.Open();

                    var trans = conn.BeginTransaction();
                    bool er = false;
                    FunctionString Fns = new FunctionString(connectionString);
                    DynamicParameters spParam = new DynamicParameters();
                    //Dictionary<string, string> ObjOutput = new Dictionary<string, string>();
                    JObject ObjOutput = new JObject();
                    JObject OutObj = new JObject();
                    try
                    {
                        Object affectedRows = null;
                        data = Fns.DecryptData(data);
                        spParam = Fns.SpParameters(data, OptionRepo.getListParamType(MvSpName));
                        affectedRows = conn.Query(SpName, spParam, commandType: CommandType.StoredProcedure);
                        #region Data Delete 2
                        if (data["_Delete2_"] != null)
                        {
                            JObject Jdata2 = new JObject();
                            foreach (var x in data["_Delete2_"].ToArray())
                            {
                                Jdata2 = JObject.Parse(x.ToString());
                            }
                            if (Jdata2["_MethodApi_"] == null)
                            {
                                OutObj.Add("No", n);
                                OutObj.Add("Data", "");
                                OutObj.Add("Error", true);
                                OutObj.Add("Message", "_Delete2_ : Paramter _MethodApi_ can't be Empty");
                                trans.Rollback();
                                er = true;

                                //return;
                            }
                            else if (Jdata2["_LineNo_"] == null)
                            {
                                OutObj.Add("No", n);
                                OutObj.Add("Data", "");
                                OutObj.Add("Error", true);
                                OutObj.Add("Message", "_Delete2_ : Paramter _LineNo_ can't be Empty");
                                trans.Rollback();
                                er = true;
                                /*return*/
                                ;
                            }
                            else
                            {
                                spParam = new DynamicParameters();
                                string MethodAPi = Jdata2["_MethodApi_"].ToString();
                                int line_no = Convert.ToInt32(Jdata2["_LineNo_"]);
                                string SpName2 = (from op in OptionRepo.GetListUrl(JModel.option_url).Where(x => x.method_api.ToUpper() == MethodAPi.ToString().ToUpper() && x.line_no == line_no)
                                                  select op.sp).FirstOrDefault();

                                if (SpName2 == null)
                                {
                                    OutObj.Add("No", n);
                                    OutObj.Add("Data", "");
                                    OutObj.Add("Error", true);
                                    OutObj.Add("Message", "No Data For Method : " + MethodAPi + " AND line_no :" + line_no.ToString());
                                    trans.Rollback();
                                    er = true;
                                    //return;
                                }
                                else
                                {
                                    Object affectedRow2s = null;
                                    Jdata2 = Fns.DecryptData(Jdata2);
                                    spParam = Fns.SpParameters(Jdata2, OptionRepo.getListParamType(SpName2));
                                    SpName2 = "" + SpName2 + "";
                                    affectedRow2s = conn.Execute(SpName2, spParam, commandType: CommandType.StoredProcedure, transaction: trans);
                                }


                            }

                        }
                        #endregion

                        if (!er)
                        {
                            if (data["_Message_"] != null)
                                ObjOutput.Add("Message", "");

                            OutObj.Add("No", n);
                            OutObj.Add("Data", ObjOutput);
                            OutObj.Add("Error", false);
                            OutObj.Add("Message", "");

                            trans.Commit();
                        }

                    }
                    catch (Exception ex)
                    {
                        //_result = Tools.Error(ex);
                        OutObj.Add("No", n);
                        OutObj.Add("Data", "");
                        OutObj.Add("Error", true);
                        if (ex.Message.IndexOf("\r\n") > 0)
                        {
                            OutObj.Add("Message", ex.Message.Substring(0, ex.Message.IndexOf("\r\n")));
                        }
                        else
                        {
                            OutObj.Add("Message", ex.Message);
                        }
                        //er = false;
                        trans.Rollback();
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open) conn.Close();
                    }
                    LOutPut.Add(OutObj);
                });
                _result.Data = LOutPut;

            }

            return _result;
        }

        public Output executeMultiPost(DynamicMultiParam JModel, SQL.Method.Aggregate Method)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {

                if (JModel.Data.Count == 0 || string.IsNullOrEmpty(JModel.option_url))
                {
                    throw new Exception("Data not Valid.");
                }

                List<JObject> LOutPut = new List<JObject>();

                MvSpName = (from OD in OptionRepo.GetListUrl(JModel.option_url).Where(x => x.method_api.ToUpper() == Method.ToString().ToUpper() && x.line_no == JModel.line_no)
                            select OD.sp).FirstOrDefault();

                if (string.IsNullOrEmpty(MvSpName))
                {
                    throw new Exception("Please Contact Your Administrator (Table setting null or Empty).");
                }

                string SpName = MvSpName;
                int n = 1;
                JModel.Data.ForEach(delegate (JObject data)
                {
                    if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                        conn.Open();

                    var trans = conn.BeginTransaction();
                    bool er = false;
                    FunctionString Fns = new FunctionString(connectionString);
                    DynamicParameters spParam = new DynamicParameters();
                    //Dictionary<string, string> ObjOutput = new Dictionary<string, string>();
                    JObject ObjOutput = new JObject();
                    JObject OutObj = new JObject();
                    try
                    {
                        Object affectedRows = null;
                        data = Fns.DecryptData(data);
                        spParam = Fns.SpParameters(data, OptionRepo.getListParamType(MvSpName));
                        affectedRows = conn.Query(SpName, spParam, commandType: CommandType.StoredProcedure);



                        if (data["_Message_"] != null)
                            //ObjOutput.Add("Message", JObject.Parse("",affectedRows));

                            OutObj.Add("No", n);
                        OutObj.Add("Data", ObjOutput);
                        OutObj.Add("Error", false);
                        OutObj.Add("Message", "");

                        trans.Commit();


                    }
                    catch (Exception ex)
                    {
                        //_result = Tools.Error(ex);
                        OutObj.Add("No", n);
                        OutObj.Add("Data", "");
                        OutObj.Add("Error", true);
                        if (ex.Message.IndexOf("\r\n") > 0)
                        {
                            OutObj.Add("Message", ex.Message.Substring(0, ex.Message.IndexOf("\r\n")));
                        }
                        else
                        {
                            OutObj.Add("Message", ex.Message);
                        }
                        //er = false;
                        trans.Rollback();
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open) conn.Close();
                    }
                    LOutPut.Add(OutObj);
                });
                _result.Data = LOutPut;

            }

            return _result;
        }

        public Output executeMulti(DynamicMultiParam JModel, SQL.Method.Aggregate Method)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {

                if (JModel.Data.Count == 0 || string.IsNullOrEmpty(JModel.option_url))
                {
                    throw new Exception("Data not Valid.");
                }

                List<JObject> LOutPut = new List<JObject>();

                MvSpName = (from OD in OptionRepo.GetListUrl(JModel.option_url).Where(x => x.method_api.ToUpper() == Method.ToString().ToUpper() && x.line_no == JModel.line_no)
                            select OD.sp).FirstOrDefault();

                if (string.IsNullOrEmpty(MvSpName))
                {
                    throw new Exception("Please Contact Your Administrator (Table setting null or Empty).");
                }

                string SpName = MvSpName;
                int n = 1;
                JModel.Data.ForEach(delegate (JObject data)
                {
                    if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                        conn.Open();

                    var trans = conn.BeginTransaction();
                    bool er = false;
                    FunctionString Fns = new FunctionString(connectionString);
                    DynamicParameters spParam = new DynamicParameters();
                    //Dictionary<string, string> ObjOutput = new Dictionary<string, string>();
                    JObject ObjOutput = new JObject();
                    JObject OutObj = new JObject();
                    try
                    {
                        Object affectedRows = null;
                        data = Fns.DecryptData(data);
                        spParam = Fns.SpParameters(data, OptionRepo.getListParamType(MvSpName));
                        affectedRows = conn.Query(SpName, spParam, commandType: CommandType.StoredProcedure);

                        if (!er)
                        {
                            if (data["_Message_"] != null)
                                ObjOutput.Add("Message", "");

                            OutObj.Add("No", n);
                            OutObj.Add("Data", ObjOutput);
                            OutObj.Add("Error", false);
                            OutObj.Add("Message", "");

                            trans.Commit();
                        }

                    }
                    catch (Exception ex)
                    {
                        //_result = Tools.Error(ex);
                        OutObj.Add("No", n);
                        OutObj.Add("Data", "");
                        OutObj.Add("Error", true);
                        if (ex.Message.IndexOf("\r\n") > 0)
                        {
                            OutObj.Add("Message", ex.Message.Substring(0, ex.Message.IndexOf("\r\n")));
                        }
                        else
                        {
                            OutObj.Add("Message", ex.Message);
                        }
                        trans.Rollback();
                        _result.Error = true;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open) conn.Close();
                    }
                    LOutPut.Add(OutObj);
                });
                _result.Data = LOutPut;

            }

            return _result; ;
        }

        public Output ProsesDataPostMulti(DynamicMultiParam JModel, SQL.Method.Aggregate Method)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                    conn.Open();

                var trans = conn.BeginTransaction();
                try
                {
                    if (JModel.Data.Count == 0)
                    {
                        throw new Exception("Data not valid.");
                    }

                    List<OptionDB> OptionDB = OptionRepo.GetListUrl(JModel.option_url);
                    JObject dataParamOutput = new JObject();

                    bool sError = false;
                    JModel.Data.ForEach(delegate (JObject val)
                    {
                        var vvv = val;
                        foreach (JProperty x in val.Properties())
                        {
                            JObject isiData = new JObject();
                            Object affectedRows = null;
                            DynamicParameters spParam = new DynamicParameters();
                            var Name = x.Name.ToString().Trim();

                            #region data single
                            if (!Name.ToUpper().Contains("LOOPING"))
                            {
                                isiData = JObject.Parse(x.Value.ToString());
                                string MethodIsi = isiData["_Method_"].ToString();
                                int LineNoIsi = Convert.ToInt32(isiData["_LineNo_"]);
                                string SPName = OptionDB.Where(w => w.line_no == LineNoIsi && w.method_api == MethodIsi).Select(s => s.sp).FirstOrDefault();

                                isiData = fn.DecryptData(isiData);
                                if (dataParamOutput.Count > 0)
                                {
                                    isiData = fn.SetParamWithValueOutput(isiData, dataParamOutput);
                                }

                                spParam = fn.SpParameters(isiData, OptionRepo.getListParamType(SPName));
                                var dt = conn.Query(SPName, spParam, commandType: CommandType.StoredProcedure);

                                if (dt.Count() > 0)
                                {
                                    var ddd = dt.ToList()[0];
                                    dataParamOutput.Add(Name, JObject.FromObject(ddd));
                                }

                            }
                            #endregion
                            #region data Looping
                            else
                            {
                                int indexs = 0;
                                foreach (var xx in x.Value.ToArray())
                                {
                                    isiData = new JObject();
                                    isiData = JObject.Parse(xx.ToString());

                                    string MethodIsi = isiData["_Method_"].ToString();
                                    int LineNoIsi = Convert.ToInt32(isiData["_LineNo_"]);

                                    string SPName = OptionDB.Where(w => w.line_no == LineNoIsi && w.method_api == MethodIsi).Select(s => s.sp).FirstOrDefault();
                                    if (string.IsNullOrEmpty(SPName))
                                    {
                                        _result.Error = true;
                                        _result.Message = "SPName can't be null.";
                                        trans.Rollback();
                                        sError = true;
                                        //return OutputAPi;
                                        continue;
                                    }
                                    isiData = fn.DecryptData(isiData);
                                    if (dataParamOutput.Count > 0)
                                    {
                                        isiData = fn.SetParamWithValueOutput(isiData, dataParamOutput);
                                    }
                                    spParam = fn.SpParameters(isiData, OptionRepo.getListParamType(SPName));
                                    var dt = conn.Query(SPName, spParam, commandType: CommandType.StoredProcedure);
                                    if (dt.Count() > 0)
                                    {
                                        var ddd = dt.ToList()[0];
                                        dataParamOutput.Add(Name + indexs.ToString(), JObject.FromObject(ddd));
                                    }
                                    indexs += 1;
                                }

                            }
                            #endregion
                        }
                    });
                    if (!sError)
                    {
                        trans.Commit();
                    }
                    _result.Data = dataParamOutput;

                    _result.Message = "";
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    _result = Tools.Error(ex);
                    //_result.Error = true;
                    //_result.Message = ex.Message;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return _result;
        }

        public DTResultListLookup<dynamic> GetlistLookUp(ParamLookupList JModel, SQL.Method.Aggregate Method)
        {
            var OutputList = new DTResultListLookup<dynamic>();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {

                    string sWhere = string.Empty;
                    //string user_id = fn.DecryptString(JModel["user_id"].ToString());
                    //string subportfolio_id = JModel["subportfolio_id"].ToString();
                    //string portfolio_cd = JModel["portfolio_cd"].ToString();

                    var ParamView = JModel.param_view == null ? string.Empty : JModel.param_view;
                    bool isViewFunction = !string.IsNullOrEmpty(ParamView) ? true : false;
                    //int JmlahField = config.GetValue<int>("appSetting:JumlahFieldList");

                    int iStart = JModel.current_page;
                    int iPerPage = JModel.per_page;
                    int JmlahField = config.GetValue<int>("appSetting:JumlahFieldList");

                    DynamicParameters spParam = new DynamicParameters();
                    var LookUpCd = JModel.look_up_cd;
                    var ColumnDB = JModel.column_db;

                    var OptionLookUp = OptionRepo.GetdataLookUp(LookUpCd, ColumnDB);

                    if (OptionLookUp == null)
                    {
                        throw new Exception("Please Contact Your Administrator (Table setting Lookup null or Empty).");
                    }

                    //string source_field = JModel.SourceField"] == null ? OptionLookUp.source_field : JModel["SourceField"].ToString();
                    string source_field = OptionLookUp.source_field;
                    string sort = !string.IsNullOrEmpty(source_field) ? source_field.Split(",")[0] + " ASC" : JModel.sort_field;
                    string sSortField = string.IsNullOrEmpty(sort) ? string.Empty : "ORDER BY " + sort;

                    MvSpName = OptionLookUp.view_name;//string.IsNullOrEmpty(ParamView) ? OptionLookUp.view_name : string.Format("{0}({1})", OptionLookUp.view_name, ParamView);
                    #region Field
                    var fieldsource = OptionRepo.getListFieldType(MvSpName, isViewFunction);
                    if (fieldsource.Count == 0)
                    {
                        throw new Exception("Please Contact Your Administrator (Table setting Lookup null or Empty).");
                    }
                    MvSpName = string.IsNullOrEmpty(ParamView) ? OptionLookUp.view_name : string.Format("{0}({1})", OptionLookUp.view_name, ParamView);
                    JObject dataFieldList = fn.SetFieldList(fieldsource, JmlahField, definedColumn: source_field);
                    string AllColumn = OptionLookUp.source_field;//fn.SetFieldList(fieldsource, fieldsource.Count)["Field"].ToString();
                    string allCoulumnQUery = dataFieldList["FieldQuery"].ToString();
                    string DefineSize = dataFieldList["DefineSize"].ToString();
                    //string AllColumn = OptionLookUp.source_field;
                    //cek defineColumn dlu
                    #endregion

                    #region Parameter Where
                    var ParamWHere = JModel.param_where;
                    var initialwhere = JModel.initial_where;
                    initialwhere = string.IsNullOrEmpty(initialwhere) ? initialwhere : "WHERE " + initialwhere;
                    sWhere = fn.DecryptDataString(initialwhere);

                    sWhere += !string.IsNullOrEmpty(ParamWHere) ? !string.IsNullOrEmpty(sWhere) ? " AND " + fn.sWhereLikeList(AllColumn, ParamWHere) : fn.sWhereLikeList(AllColumn, ParamWHere) : string.Empty;
                    #endregion


                    //conn.Open();

                    allCoulumnQUery = allCoulumnQUery + ",'|' as separator";

                    OutputList.Data = this.QueryList(MvSpName, iStart, iPerPage, sSortField, sWhere, allCoulumnQUery); //DefineColumnFormat
                    OutputList.Total = Convert.ToInt32(this.CountList(MvSpName, sWhere));
                    OutputList.Current_Page = iStart;
                    OutputList.Last_Page = Convert.ToInt32(Math.Ceiling((Convert.ToDecimal(OutputList.Total) / iPerPage)));

                    OutputList.AllColumn = source_field;
                }
                catch (Exception ex)
                {
                    OutputList.Error = true;
                    OutputList.Message = ex.Message;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return OutputList;
        }

        public Output GetDataLookUp(JObject JModel, SQL.Method.Aggregate Method)
        {
            var OP = new Output();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    if (JModel.Count == 0)
                    {
                        throw new Exception("Data no valid.");
                    }



                    DynamicParameters spParam = new DynamicParameters();
                    var LookUpCd = JModel["LookUpCd"].ToString();
                    var ColumnDB = JModel["ColumnDB"].ToString();

                    var ParamView = JModel["ParamView"] == null ? string.Empty : JModel["ParamView"].ToString();
                    bool isViewFunction = !string.IsNullOrEmpty(ParamView) ? true : false;


                    var OptionLookUp = OptionRepo.GetdataLookUp(LookUpCd, ColumnDB);

                    if (OptionLookUp == null)
                    {
                        throw new Exception("Please Contact Your Administrator (Table setting Lookup null or Empty).");
                    }


                    string source_field = fn.SetFieldListLookup(OptionLookUp.source_field, OptionLookUp.display_lookup);


                    MvSpName = string.IsNullOrEmpty(ParamView) ? OptionLookUp.view_name : string.Format("{0}({1})", OptionLookUp.view_name, ParamView);
                    #region Parameter Where
                    var ParamWHere = JModel["ParamWhere"].ToString();
                    var initialwhere = JModel["InitialWhere"].ToString();
                    initialwhere = string.IsNullOrEmpty(initialwhere) ? initialwhere : "WHERE " + initialwhere;
                    string sWhere = fn.DecryptDataString(initialwhere);

                    sWhere += !string.IsNullOrEmpty(ParamWHere) ? !string.IsNullOrEmpty(sWhere) ? " AND " + fn.sWhereLikeList(OptionLookUp.source_field, ParamWHere) : fn.sWhereLikeList(OptionLookUp.source_field, ParamWHere) : string.Empty;
                    #endregion


                    OP.Data = this.QueryList(MvSpName, sWhere, sField: source_field, Limit: "10000");
                }
                catch (Exception ex)
                {
                    OP = Tools.Error(ex);
                    //OP.Error = true;
                    //OP.Message = ex.Message;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }

            return OP;
        }

        public Output GetDataByLookUp(JObject JModel, SQL.Method.Aggregate Method)
        {
            throw new NotImplementedException();
        }

        public Output executeFunction(JObject JModel)
        {
            var OutputList = new Output();
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    if (JModel.Count == 0)
                    {
                        throw new Exception("Data no valid.");
                    }
                    conn.Open();
                    DynamicParameters spParam = new DynamicParameters();
                    if (JModel["option_function_cd"] == null)
                    {
                        throw new Exception("Parameter option_function_cd can't be null");
                    }

                    if (JModel["module_cd"] == null)
                    {
                        throw new Exception("Parameter module_cd can't be null");
                    }

                    var option_function_cd = JModel["option_function_cd"].ToString();
                    var ModuleCd = string.Empty;


                    if (JModel["module_cdv2"] != null)
                    { ModuleCd = JModel["module_cdv2"].ToString(); }
                    else { ModuleCd = JModel["module_cd"].ToString(); }

                    if (JModel["module_cdv2"] != null)
                    { JModel.Remove("module_cdv2"); }
                    else { JModel.Remove("module_cd"); }

                    MvSpName = (from OD in OptionRepo.GetListOptFunction(option_function_cd, ModuleCd)
                                select OD.sp_name).FirstOrDefault();

                    if (string.IsNullOrEmpty(MvSpName))
                    {
                        throw new Exception("Please Contact Your Administrator (Table function null or Empty).");
                    }
                    JModel.Remove("option_function_cd");

                    JModel = fn.DecryptData(JModel);
                    spParam = fn.SpParameters(JModel, OptionRepo.getListParamType(MvSpName));
                    var datas = conn.Query(MvSpName, spParam, commandType: CommandType.StoredProcedure);
                    OutputList.Data = datas;

                }
                catch (Exception ex)
                {
                    //throw ex;
                    OutputList = Tools.Error(ex);
                    //OutputList.Error = true;// Tools.Error(ex);
                    //OutputList.Message = ex.Message;
                }
            }

            return OutputList;
        }

        public Output GetDataSpec(JObject JModel)
        {
            throw new NotImplementedException();
        }
        public List<dynamic> QueryList(string sTable, int iStart, int iPageSize, string sSortField, string sParameter, string sField = "")
        {
            List<dynamic> op = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    sField = string.IsNullOrEmpty(sField) ? "*" : sField;
                    int iOffset = (iStart * iPageSize) - iPageSize;
                    sParameter = string.IsNullOrEmpty(sParameter) ? sParameter : sParameter.ToUpper().Contains("WHERE") ? sParameter : "WHERE " + sParameter;
                    //sParameter = fn.FormatWhere(sParameter);
                    StringBuilder sQuery = new StringBuilder();
                    sQuery.AppendFormat(" WITH result_set AS ");
                    sQuery.AppendFormat(" ( ");
                    sQuery.AppendFormat("SELECT  ", sField, sTable);
                    sQuery.AppendFormat(" row_number() OVER ({0}) as no,  ", sSortField);
                    sQuery.AppendFormat(" {0} ", sField);
                    sQuery.AppendFormat(" FROM  {0} ", sTable);
                    sQuery.AppendFormat(" {0} ", sParameter);
                    sQuery.AppendFormat(" ) ");
                    sQuery.AppendFormat(" SELECT * FROM  result_set ", sSortField);
                    sQuery.AppendFormat(" LIMIT {0} ", iPageSize);
                    sQuery.AppendFormat(" OFFSET {0} ;", iOffset);
                    conn.Open();
                    op = conn.Query<dynamic>(sQuery.ToString()).ToList();
                }
                catch (Exception ex)
                {
                    //op = Helper.Error(ex);
                    throw ex;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }

            return op;
        }
        public List<dynamic> QueryList(string sTable, string sParameter, string sField = "", string Limit = "")
        {
            List<dynamic> op = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    sField = string.IsNullOrEmpty(sField) ? "*" : sField;

                    sParameter = string.IsNullOrEmpty(sParameter) ? sParameter : sParameter.ToUpper().Contains("WHERE") ? sParameter : "WHERE " + sParameter;
                    StringBuilder sQuery = new StringBuilder();
                    sQuery.AppendFormat("SELECT distinct {0} FROM {1} ", sField, sTable);
                    sQuery.AppendFormat(" {0} ", sParameter);
                    if (!string.IsNullOrEmpty(Limit))
                    {
                        sQuery.AppendFormat(" LIMIT {0}; ", Limit);
                    }

                    conn.Open();
                    op = conn.Query<dynamic>(sQuery.ToString()).ToList();
                }
                catch (Exception ex)
                {
                    //op = Helper.Error(ex);
                    throw ex;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }

            return op;
        }
        public object CountList(string sTable, string sParameter)
        {
            object op = null;
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                try
                {
                    sParameter = !string.IsNullOrEmpty(sParameter) ? sParameter.ToLower().Contains("where") ? sParameter : "WHERE " + sParameter : sParameter;
                    //sParameter = fn.FormatWhere(sParameter);
                    StringBuilder sQuery = new StringBuilder();
                    sQuery.AppendFormat("SELECT COUNT(*) FROM {0} {1}", sTable, sParameter);
                    conn.Open();
                    op = conn.ExecuteScalar(sQuery.ToString());
                }
                catch (Exception ex)
                {
                    //op = Helper.Error(ex);
                    throw ex;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }

            return op;
        }
    }
}
