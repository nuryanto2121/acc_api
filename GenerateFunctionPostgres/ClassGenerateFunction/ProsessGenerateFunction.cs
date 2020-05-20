using GenerateFunctionPostgres.Models;
using GenerateFunctionPostgres.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GenerateFunctionPostgres.ClassGenerateFunction
{
    public class ProsessGenerateFunction
    {
        public string PathError;
        private string BodyTemplateFunction = string.Empty;
        private string TableName = string.Empty;
        private string ParameterInsert = string.Empty;
        private string FieldInsert = string.Empty;
        private string FieldInsertParameter = string.Empty;
        private string ParameterUpdate = string.Empty;
        private string FieldUpdateParameter = string.Empty;
        private string WhereUpdate = string.Empty;
        private string ReturnTable = string.Empty;
        private string QueryTable = string.Empty;

        private string space = string.Empty;

        public FunctionPostgres fnP;// = new FunctionPostgres();
        public TableHeaderRepoNpgs tHeadRepo;// = new TableHeaderRepoNpgs();

        public ProsessGenerateFunction(string ConnectionString)
        {
            fnP = new FunctionPostgres(ConnectionString);
            tHeadRepo = new TableHeaderRepoNpgs(ConnectionString);
        }
        public bool CreateFileFunction(table_header dataHeader, string FolderPath)
        {
            bool _result = false;

            try
            {
                List<ParameterPostgres> PropertiesTable = fnP.GetPropertiesTable(dataHeader.table_name);
                List<table_detail> DataDetail = tHeadRepo.GetListDetailById(dataHeader.header_id);

                string PathFile = Path.Combine(/*Application.StartupPath*/"", "TemplateMasterVue", "MasterCrudPostgres.sql");//Application.StartupPath + "\\TemplateMasterVue\\SaveFirst.vue";
                using (StreamReader reader = new StreamReader(PathFile))
                {
                    BodyTemplateFunction = reader.ReadToEnd();
                }
                BodyTemplateFunction = BodyTemplateFunction.Replace("{TableName}", dataHeader.table_name);

                #region script Insert
                ParameterInsert = fn_ParameterFunction(PropertiesTable, "I");
                FieldInsert = fn_FieldInsert(PropertiesTable);
                FieldInsertParameter = fn_FieldInsertParameter(PropertiesTable);

                BodyTemplateFunction = BodyTemplateFunction.Replace("{ParameterInsert}", ParameterInsert);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{FieldInsert}", FieldInsert);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{FieldInsertParameter}", FieldInsertParameter);
                #endregion

                #region script Update
                ParameterUpdate = fn_ParameterFunction(PropertiesTable, "U");
                FieldUpdateParameter = fn_FieldUpdate(PropertiesTable);
                WhereUpdate = fn_FieldInsertParameter(PropertiesTable);

                BodyTemplateFunction = BodyTemplateFunction.Replace("{ParameterUpdate}", ParameterUpdate);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{FieldUpdateParameter}", FieldUpdateParameter);
                #endregion

                #region scrtp select
                ReturnTable = fn_ReturnTable(DataDetail, PropertiesTable);
                QueryTable = fn_QueryString(DataDetail, PropertiesTable);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{ReturnTable}", ReturnTable);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{QueryTable}", QueryTable);
                #endregion

                //fnP.ExecQuery(BodyTemplateFunction);
                var PathComponent = Path.Combine(FolderPath, string.Format("CRUD_{0}.sql", dataHeader.table_name));
                string[] HtmlPageMaster = BodyTemplateFunction.Split(
                                   new[] { Environment.NewLine },
                                   StringSplitOptions.None
                               );
                Tools.Helper.writeFile(HtmlPageMaster, PathComponent);
                _result = true;
            }
            catch (Exception ex)
            {
                var errorFolder = FolderPath + "\\ErrorLog";
                var ErrorPath = errorFolder + "/" + DateTime.Now.ToString("ddMMyyyyhhmm") + "_ErrorLogDetail.txt";
                if (!Directory.Exists(errorFolder))
                {
                    Directory.CreateDirectory(errorFolder);
                }
                string err = dataHeader.url + " : " + dataHeader.title + Environment.NewLine + ex.Message + " - " + ex.StackTrace;
                string[] Errs = err.Split(
                                    new[] { Environment.NewLine },
                                    StringSplitOptions.None
                                );
                Tools.Helper.writeFile(Errs, ErrorPath);
                throw (ex);
            }
            return _result;
        }
        public string CreateFileFunction(int HeaderID, string FolderPath, string PathRoot)
        {

            string _result = string.Empty;
            try
            {
                string sWhere = string.Format("header_id = {0}", HeaderID);
                table_header dataHeader = tHeadRepo.GetListBy(sWhere).FirstOrDefault();

                List<ParameterPostgres> PropertiesTable = fnP.GetPropertiesTable(dataHeader.table_name);
                List<table_detail> DataDetail = tHeadRepo.GetListDetailById(dataHeader.header_id);

                string PathFile = Path.Combine(PathRoot, "Template", "MasterCrudPostgres.txt");

                if (dataHeader.form_type.ToUpper() == "FORM")
                {
                    PathFile = Path.Combine(PathRoot, "Template", "MasterCrudPostgresForm.txt");
                }
                using (StreamReader reader = new StreamReader(PathFile))
                {
                    BodyTemplateFunction = reader.ReadToEnd();
                }

                ///*BodyTemplateFunction*/ = TemplateString.MasterCrudPostgres();
                BodyTemplateFunction = BodyTemplateFunction.Replace("{TableName}", dataHeader.table_name);

                #region script Insert
                ParameterInsert = fn_ParameterFunction(PropertiesTable, "I");
                FieldInsert = fn_FieldInsert(PropertiesTable);
                FieldInsertParameter = fn_FieldInsertParameter(PropertiesTable);

                BodyTemplateFunction = BodyTemplateFunction.Replace("{ParameterInsert}", ParameterInsert);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{FieldInsert}", FieldInsert);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{FieldInsertParameter}", FieldInsertParameter);
                #endregion

                #region script Update
                ParameterUpdate = fn_ParameterFunction(PropertiesTable, "U");
                FieldUpdateParameter = fn_FieldUpdate(PropertiesTable);
                WhereUpdate = fn_FieldInsertParameter(PropertiesTable);

                BodyTemplateFunction = BodyTemplateFunction.Replace("{ParameterUpdate}", ParameterUpdate);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{FieldUpdateParameter}", FieldUpdateParameter);
                #endregion

                #region script select
                ReturnTable = fn_ReturnTable(DataDetail, PropertiesTable);
                QueryTable = fn_QueryString(DataDetail, PropertiesTable, true, dataHeader.form_type.ToUpper() == "FORM", dataHeader.relation_param);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{ReturnTable}", ReturnTable);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{QueryTable}", QueryTable);

                if (dataHeader.form_type.ToUpper() == "FORM")
                {
                    string paramForForm = string.Format("p_{0} {1}", dataHeader.relation_param, dataHeader.relation_type);
                    BodyTemplateFunction = BodyTemplateFunction.Replace("{ParamGetDataBy}", paramForForm);
                }
                #endregion

                #region script view
                QueryTable = fn_QueryString(DataDetail, PropertiesTable);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{QueryTableView}", QueryTable);
                #endregion

                //fnP.ExecQuery(BodyTemplateFunction);
                var PathComponent = Path.Combine(FolderPath, string.Format("CRUD_{0}.sql", dataHeader.table_name));
                string[] HtmlPageMaster = BodyTemplateFunction.Split(
                                   new[] { Environment.NewLine },
                                   StringSplitOptions.None
                               );
                Tools.Helper.writeFile(HtmlPageMaster, PathComponent);
                _result = string.Format("CRUD_{0}.sql", dataHeader.table_name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public string CreateFileFunction(string TableName, string FolderPath, string PathRoot)
        {

            string _result = string.Empty;
            try
            {
                //string sWhere = string.Format("header_id = {0}", TableName);
                //table_header dataHeader = tHeadRepo.GetListBy(sWhere).FirstOrDefault();

                List<ParameterPostgres> PropertiesTable = fnP.GetPropertiesTable(TableName);
                //List<table_detail> DataDetail = tHeadRepo.GetListDetailById(dataHeader.header_id);

                string PathFile = Path.Combine(PathRoot, "Template", "MasterCrudPostgres.txt");

                //if (dataHeader.form_type.ToUpper() == "FORM")
                //{
                //    PathFile = Path.Combine(PathRoot, "Template", "MasterCrudPostgresForm.txt");
                //}
                using (StreamReader reader = new StreamReader(PathFile))
                {
                    BodyTemplateFunction = reader.ReadToEnd();
                }

                ///*BodyTemplateFunction*/ = TemplateString.MasterCrudPostgres();
                BodyTemplateFunction = BodyTemplateFunction.Replace("{TableName}", TableName);

                #region script Insert
                ParameterInsert = fn_ParameterFunction(PropertiesTable, "I");
                FieldInsert = fn_FieldInsert(PropertiesTable);
                FieldInsertParameter = fn_FieldInsertParameter(PropertiesTable);

                BodyTemplateFunction = BodyTemplateFunction.Replace("{ParameterInsert}", ParameterInsert);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{FieldInsert}", FieldInsert);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{FieldInsertParameter}", FieldInsertParameter);
                #endregion

                #region script Update
                ParameterUpdate = fn_ParameterFunction(PropertiesTable, "U");
                FieldUpdateParameter = fn_FieldUpdate(PropertiesTable);
                WhereUpdate = fn_FieldInsertParameter(PropertiesTable);

                BodyTemplateFunction = BodyTemplateFunction.Replace("{ParameterUpdate}", ParameterUpdate);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{FieldUpdateParameter}", FieldUpdateParameter);
                #endregion

                #region script select
                ReturnTable = fn_ReturnTable(PropertiesTable);
                QueryTable = fn_QueryString(PropertiesTable);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{ReturnTable}", ReturnTable);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{QueryTable}", QueryTable);

                //if (dataHeader.form_type.ToUpper() == "FORM")
                //{
                //    string paramForForm = string.Format("p_{0} {1}", dataHeader.relation_param, dataHeader.relation_type);
                //    BodyTemplateFunction = BodyTemplateFunction.Replace("{ParamGetDataBy}", paramForForm);
                //}
                #endregion

                #region script view
                QueryTable = fn_QueryString(PropertiesTable);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{QueryTableView}", QueryTable);
                #endregion

                //fnP.ExecQuery(BodyTemplateFunction);
                var PathComponent = Path.Combine(FolderPath, string.Format("CRUD_{0}.sql", TableName));
                string[] HtmlPageMaster = BodyTemplateFunction.Split(
                                   new[] { Environment.NewLine },
                                   StringSplitOptions.None
                               );
                Tools.Helper.writeFile(HtmlPageMaster, PathComponent);
                _result = string.Format("CRUD_{0}.sql", TableName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public string fn_ParameterFunction(List<ParameterPostgres> PropertiesTable, string status)
        {
            string _result = string.Empty;
            try
            {

                foreach (ParameterPostgres data in PropertiesTable)
                {
                    string DataType = data.data_type == "character varying" ? "varchar" : data.data_type == "timestamp without time zone" ? "timestamp" : data.data_type;
                    if (status == "I")
                    {
                        if (data.default_value.Contains("nextval("))
                            continue;
                        if (data.column_name.Contains("user_edit"))
                            continue;
                        if (data.column_name.Contains("time_input"))
                            continue;
                        if (data.column_name.Contains("time_edit"))
                            continue;

                        _result += space.PadRight(5) + string.Format("p_{0} {1},", data.column_name, DataType) + Environment.NewLine;
                    }
                    else
                    {


                        if (data.column_name.Contains("time_input"))
                            continue;
                        if (data.column_name.Contains("time_edit"))
                            continue;

                        if (data.column_name.Contains("user_input"))
                        {
                            _result += space.PadRight(5) + string.Format("p_lastupdatestamp integer,") + Environment.NewLine;
                            continue;
                        }

                        _result += space.PadRight(5) + string.Format("p_{0} {1},", data.column_name, DataType) + Environment.NewLine;
                    }


                }
                _result = !string.IsNullOrEmpty(_result) ? _result.Remove(_result.LastIndexOf(',')) : _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public string fn_FieldInsert(List<ParameterPostgres> PropertiesTable)
        {
            string _result = string.Empty;
            try
            {

                foreach (ParameterPostgres data in PropertiesTable)
                {
                    if (data.default_value.Contains("nextval("))
                        continue;

                    if (data.column_name.Contains("time_input"))
                        continue;
                    if (data.column_name.Contains("time_edit"))
                        continue;

                    _result += space.PadRight(10) + string.Format("{0}, ", data.column_name) + Environment.NewLine;

                }
                _result = !string.IsNullOrEmpty(_result) ? _result.Remove(_result.LastIndexOf(',')) : _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public string fn_FieldInsertParameter(List<ParameterPostgres> PropertiesTable)
        {
            string _result = string.Empty;
            try
            {

                foreach (ParameterPostgres data in PropertiesTable)
                {
                    if (data.default_value.Contains("nextval("))
                        continue;
                    if (data.column_name.Contains("time_input"))
                        continue;
                    if (data.column_name.Contains("time_edit"))
                        continue;

                    if (data.column_name.Contains("user_edit"))
                    {
                        _result += space.PadRight(10) + string.Format("p_user_input,") + Environment.NewLine;
                    }
                    else
                    {
                        _result += space.PadRight(10) + string.Format("p_{0},", data.column_name) + Environment.NewLine;
                    }



                }
                _result = !string.IsNullOrEmpty(_result) ? _result.Remove(_result.LastIndexOf(',')) : _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public string fn_FieldUpdate(List<ParameterPostgres> PropertiesTable)
        {
            string _result = string.Empty;
            try
            {

                foreach (ParameterPostgres data in PropertiesTable)
                {
                    if (data.default_value.Contains("nextval("))
                        continue;

                    if (data.column_name.Contains("user_input"))
                        continue;
                    if (data.column_name.Contains("time_input"))
                        continue;

                    if (data.column_name.Contains("time_edit"))
                    {
                        _result += space.PadRight(15) + string.Format("{0} = now()::timestamp ,", data.column_name) + Environment.NewLine;
                        continue;
                    }


                    _result += space.PadRight(15) + string.Format("{0} = p_{0},", data.column_name) + Environment.NewLine;

                }
                _result = !string.IsNullOrEmpty(_result) ? _result.Remove(_result.LastIndexOf(',')) : _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public string fn_ReturnTable(List<table_detail> TableDetail, List<ParameterPostgres> PropertiesTable)
        {
            string _result = string.Empty;
            try
            {

                foreach (table_detail data in TableDetail)
                {
                    var dataPro = PropertiesTable.Where(w => w.column_name.ToLower() == data.column_name.ToLower()).FirstOrDefault();
                    string DataType = "varchar";
                    if (dataPro != null)
                    {
                        DataType = dataPro.data_type == "character varying" ? "varchar" : dataPro.data_type == "timestamp without time zone" ? "timestamp" : dataPro.data_type;
                    }


                    if (data.column_type.ToLower().Contains("lookup"))
                    {
                        string descs = Tools.Helper.getStringFromstrStartoEnd(data.column_name, data.lookup_db_parameter, data.lookup_db_parameter);
                        // id
                        _result += space.PadRight(5) + string.Format("{0} {1},", data.column_name, DataType) + Environment.NewLine;

                        // code
                        //string decsCd = string.IsNullOrEmpty(descs) ? "" : descs;
                        string decsCd = "_cd";
                        //_result += space.PadRight(5) + string.Format("{0}{1} varchar,", data.lookup_db, decsCd) + Environment.NewLine;
                        _result += space.PadRight(5) + string.Format("{0}{1} varchar,", data.column_name, decsCd) + Environment.NewLine;

                        // descs
                        decsCd = "_descs";// string.IsNullOrEmpty(descs) ? "_descs" : "_" + descs;
                        _result += space.PadRight(5) + string.Format("{0}{1} varchar,", data.column_name, decsCd) + Environment.NewLine;
                        continue;
                    }
                    else if (data.column_type.ToLower().Contains("daterange"))
                    {
                        string[] Columns = data.column_name.Split(',');
                        _result += space.PadRight(5) + string.Format("{0} timestamp,", Columns[0]) + Environment.NewLine;
                        _result += space.PadRight(5) + string.Format("{0} timestamp,", Columns[1]) + Environment.NewLine;
                        continue;
                    }
                    //else if (dataPro.default_value.ToLower().Contains("nextval("))
                    //{
                    //    _result += space.PadRight(5) + string.Format("{0} {1},", data.column_name, DataType) + Environment.NewLine;
                    //    _result += space.PadRight(5) + string.Format("row_id {1},", data.column_name, DataType) + Environment.NewLine;
                    //}
                    else
                    {
                        string colname = data.column_name;
                        if (colname.ToLower().Contains("position"))
                        {
                            colname = string.Format("\"{0}\"", colname);
                        }

                        _result += space.PadRight(5) + string.Format("{0} {1},", colname, DataType) + Environment.NewLine;
                    }



                }
                var dataPK = PropertiesTable.Where(w => w.default_value.Contains("nextval(")).FirstOrDefault();

                _result += !string.IsNullOrEmpty(dataPK?.column_name) ? space.PadRight(5) + string.Format("row_id integer,") + Environment.NewLine : "";
                _result += space.PadRight(5) + string.Format("lastupdatestamp integer,") + Environment.NewLine;
                _result += space.PadRight(5) + string.Format("time_edit timestamp,") + Environment.NewLine;
                _result += space.PadRight(5) + string.Format("user_input varchar,") + Environment.NewLine;
                _result += space.PadRight(5) + string.Format("user_edit varchar,") + Environment.NewLine;
                _result = !string.IsNullOrEmpty(_result) ? _result.Remove(_result.LastIndexOf(',')) : _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public string fn_QueryString(List<table_detail> TableDetail, List<ParameterPostgres> PropertiesTable, bool isSelect = false, bool isForm = false, string relationParam = "")
        {
            string _result = string.Empty;
            List<string> datalookups = new List<string>();
            int cntLookup = 0;
            try
            {
                _result += space.PadRight(10) + "SELECT " + Environment.NewLine;
                //gen Field dlu
                foreach (table_detail data in TableDetail)
                {
                    //var dataPro = PropertiesTable.Where(w => w.column_name.Contains(data.column_name)).FirstOrDefault();
                    //string DataType = dataPro.data_type == "character varying" ? "varchar" : dataPro.data_type == "timestamp without time zone" ? "timestamp" : dataPro.data_type;

                    if (data.column_type.ToLower().Contains("lookup"))
                    {
                        datalookups.Add(data.column_name);
                        cntLookup += 1;

                        string descs = Tools.Helper.getStringFromstrStartoEnd(data.column_name, data.lookup_db_parameter, data.lookup_db_parameter);
                        // id
                        _result += space.PadRight(15) + string.Format("a.{0},", data.column_name) + Environment.NewLine;

                        // code
                        //string decsCd = string.IsNullOrEmpty(descs) ? "" : string.Format("AS {0}{1}", data.lookup_db, descs);
                        string decsCd = string.Format("AS {0}{1}", data.column_name, "_cd");
                        _result += space.PadRight(15) + string.Format("a{0}.{1} {2},", cntLookup, data.lookup_db, decsCd) + Environment.NewLine;

                        // descs
                        decsCd = data.column_name + "_descs";// string.IsNullOrEmpty(descs) ? data.column_name + "_descs" : data.column_name + "_" + descs;
                        _result += space.PadRight(15) + string.Format("a{0}.{1} AS {2},", cntLookup, data.lookup_db_descs, decsCd) + Environment.NewLine;
                        continue;
                    }
                    else if (data.column_type.ToLower().Contains("daterange"))
                    {
                        string[] Columns = data.column_name.Split(',');
                        _result += space.PadRight(15) + string.Format("a.{0},", Columns[0]) + Environment.NewLine;
                        _result += space.PadRight(15) + string.Format("a.{0},", Columns[1]) + Environment.NewLine;
                        continue;
                    }
                    else
                    {
                        string colname = data.column_name;
                        if (colname.ToLower().Contains("position"))
                        {
                            colname = string.Format("\"{0}\"", colname);
                        }
                        _result += space.PadRight(15) + string.Format("a.{0},", colname) + Environment.NewLine;
                    }



                }
                var dataPK = PropertiesTable.Where(w => w.default_value.Contains("nextval(")).FirstOrDefault();

                _result += !string.IsNullOrEmpty(dataPK?.column_name) ? space.PadRight(15) + string.Format("a.{0} as row_id,", dataPK.column_name) + Environment.NewLine : "";
                _result += space.PadRight(15) + string.Format("a.xmin::text::integer as lastupdatestamp ,") + Environment.NewLine;
                _result += space.PadRight(15) + string.Format("a.time_edit ,") + Environment.NewLine;
                _result += space.PadRight(15) + string.Format("a.user_input ,") + Environment.NewLine;
                _result += space.PadRight(15) + string.Format("a.user_edit ,") + Environment.NewLine;
                _result = !string.IsNullOrEmpty(_result) ? _result.Remove(_result.LastIndexOf(',')) + Environment.NewLine : _result + Environment.NewLine;

                //baru gen From dan relasi table'a    
                cntLookup = 0;
                _result += space.PadRight(10) + string.Format("FROM {0} a ", TableDetail[0].table_name) + Environment.NewLine;
                if (datalookups.Count > 0)
                {
                    foreach (string colName in datalookups)
                    {
                        cntLookup += 1;
                        var dataDetail = TableDetail.Where(w => w.column_name.Contains(colName)).FirstOrDefault();
                        _result += space.PadRight(15) + string.Format("LEFT OUTER JOIN {0} a{1}", dataDetail.lookup_table, cntLookup) + Environment.NewLine;
                        _result += space.PadRight(20) + string.Format("ON a.{0} = a{1}.{2}", dataDetail.column_name, cntLookup, dataDetail.lookup_db_parameter) + Environment.NewLine;
                    }
                }
                if (isSelect)
                {
                    //_result += space.PadRight(10) + string.Format("WHERE a.{0}_id = p_{0}_id ", TableDetail[0].table_name) + Environment.NewLine;
                    if (isForm)
                    {
                        _result += space.PadRight(10) + string.Format("WHERE a.{0} = p_{0} ", relationParam) + Environment.NewLine;
                    }
                    else
                    {
                        _result += space.PadRight(10) + string.Format("WHERE a.{0}_id = p_{0}_id ", TableDetail[0].table_name) + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public string fn_QueryString(List<ParameterPostgres> PropertiesTable)
        {
            string _result = string.Empty;
            List<string> datalookups = new List<string>();
            int cntLookup = 0;
            try
            {
                _result += space.PadRight(10) + "SELECT " + Environment.NewLine;
                //gen Field dlu
                foreach (ParameterPostgres data in PropertiesTable)
                {
                    //var dataPro = PropertiesTable.Where(w => w.column_name.Contains(data.column_name)).FirstOrDefault();
                    //string DataType = dataPro.data_type == "character varying" ? "varchar" : dataPro.data_type == "timestamp without time zone" ? "timestamp" : dataPro.data_type;

                    //if (data.data_type.ToLower().Contains("lookup"))
                    //{
                    //    datalookups.Add(data.column_name);
                    //    cntLookup += 1;

                    //    string descs = Tools.Helper.getStringFromstrStartoEnd(data.column_name, data.lookup_db_parameter, data.lookup_db_parameter);
                    //    // id
                    //    _result += space.PadRight(15) + string.Format("a.{0},", data.column_name) + Environment.NewLine;

                    //    // code
                    //    //string decsCd = string.IsNullOrEmpty(descs) ? "" : string.Format("AS {0}{1}", data.lookup_db, descs);
                    //    string decsCd = string.Format("AS {0}{1}", data.column_name, "_cd");
                    //    _result += space.PadRight(15) + string.Format("a{0}.{1} {2},", cntLookup, data.lookup_db, decsCd) + Environment.NewLine;

                    //    // descs
                    //    decsCd = data.column_name + "_descs";// string.IsNullOrEmpty(descs) ? data.column_name + "_descs" : data.column_name + "_" + descs;
                    //    _result += space.PadRight(15) + string.Format("a{0}.{1} AS {2},", cntLookup, data.lookup_db_descs, decsCd) + Environment.NewLine;
                    //    continue;
                    //}
                    //else if (data.column_type.ToLower().Contains("daterange"))
                    //{
                    //    string[] Columns = data.column_name.Split(',');
                    //    _result += space.PadRight(15) + string.Format("a.{0},", Columns[0]) + Environment.NewLine;
                    //    _result += space.PadRight(15) + string.Format("a.{0},", Columns[1]) + Environment.NewLine;
                    //    continue;
                    //}
                    //else
                    //{
                    string colname = data.column_name;
                    if (colname.ToLower().Contains("position"))
                    {
                        colname = string.Format("\"{0}\"", colname);
                    }
                    _result += space.PadRight(15) + string.Format("a.{0},", colname) + Environment.NewLine;
                    //}



                }
                var dataPK = PropertiesTable.Where(w => w.default_value.Contains("nextval(")).FirstOrDefault();

                _result += !string.IsNullOrEmpty(dataPK?.column_name) ? space.PadRight(15) + string.Format("a.{0} as row_id,", dataPK.column_name) + Environment.NewLine : "";
                _result += space.PadRight(15) + string.Format("a.xmin::text::integer as lastupdatestamp ,") + Environment.NewLine;
                //_result += space.PadRight(15) + string.Format("a.time_edit ,") + Environment.NewLine;
                //_result += space.PadRight(15) + string.Format("a.user_input ,") + Environment.NewLine;
                //_result += space.PadRight(15) + string.Format("a.user_edit ,") + Environment.NewLine;
                _result = !string.IsNullOrEmpty(_result) ? _result.Remove(_result.LastIndexOf(',')) + Environment.NewLine : _result + Environment.NewLine;

                //baru gen From dan relasi table'a    
                cntLookup = 0;
                _result += space.PadRight(10) + string.Format("FROM {0} a ", PropertiesTable[0].routine_name) + Environment.NewLine;
                //if (datalookups.Count > 0)
                //{
                //    foreach (string colName in datalookups)
                //    {
                //        cntLookup += 1;
                //        var dataDetail = TableDetail.Where(w => w.column_name.Contains(colName)).FirstOrDefault();
                //        _result += space.PadRight(15) + string.Format("LEFT OUTER JOIN {0} a{1}", dataDetail.lookup_table, cntLookup) + Environment.NewLine;
                //        _result += space.PadRight(20) + string.Format("ON a.{0} = a{1}.{2}", dataDetail.column_name, cntLookup, dataDetail.lookup_db_parameter) + Environment.NewLine;
                //    }
                //}

                //_result += space.PadRight(10) + string.Format("WHERE a.{0}_id = p_{0}_id ", PropertiesTable[0].routine_name) + Environment.NewLine;
                _result += space.PadRight(10) + string.Format("WHERE a.{0}_id = p_row_id ", PropertiesTable[0].routine_name) + Environment.NewLine;
                _result += space.PadRight(10) + string.Format("AND a.xmin::text::integer = p_lastupdatestamp  ", PropertiesTable[0].routine_name) + Environment.NewLine;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public string fn_ReturnTable(List<ParameterPostgres> PropertiesTable)
        {
            string _result = string.Empty;
            try
            {

                foreach (ParameterPostgres data in PropertiesTable)
                {
                    //var dataPro = PropertiesTable.Where(w => w.column_name.ToLower() == data.column_name.ToLower()).FirstOrDefault();
                    //string DataType = "varchar";
                    //if (dataPro != null)
                    //{
                    var DataType = data.data_type == "character varying" ? "varchar" : data.data_type == "timestamp without time zone" ? "timestamp" : data.data_type;
                    //}


                    //if (data.column_type.ToLower().Contains("lookup"))
                    //{
                    //    string descs = Tools.Helper.getStringFromstrStartoEnd(data.column_name, data.lookup_db_parameter, data.lookup_db_parameter);
                    //    // id
                    //    _result += space.PadRight(5) + string.Format("{0} {1},", data.column_name, DataType) + Environment.NewLine;

                    //    // code
                    //    //string decsCd = string.IsNullOrEmpty(descs) ? "" : descs;
                    //    string decsCd = "_cd";
                    //    //_result += space.PadRight(5) + string.Format("{0}{1} varchar,", data.lookup_db, decsCd) + Environment.NewLine;
                    //    _result += space.PadRight(5) + string.Format("{0}{1} varchar,", data.column_name, decsCd) + Environment.NewLine;

                    //    // descs
                    //    decsCd = "_descs";// string.IsNullOrEmpty(descs) ? "_descs" : "_" + descs;
                    //    _result += space.PadRight(5) + string.Format("{0}{1} varchar,", data.column_name, decsCd) + Environment.NewLine;
                    //    continue;
                    //}
                    //else if (data.column_type.ToLower().Contains("daterange"))
                    //{
                    //    string[] Columns = data.column_name.Split(',');
                    //    _result += space.PadRight(5) + string.Format("{0} timestamp,", Columns[0]) + Environment.NewLine;
                    //    _result += space.PadRight(5) + string.Format("{0} timestamp,", Columns[1]) + Environment.NewLine;
                    //    continue;
                    //}
                    ////else if (dataPro.default_value.ToLower().Contains("nextval("))
                    ////{
                    ////    _result += space.PadRight(5) + string.Format("{0} {1},", data.column_name, DataType) + Environment.NewLine;
                    ////    _result += space.PadRight(5) + string.Format("row_id {1},", data.column_name, DataType) + Environment.NewLine;
                    ////}
                    //else
                    //{
                    string colname = data.column_name;
                    if (colname.ToLower().Contains("position"))
                    {
                        colname = string.Format("\"{0}\"", colname);
                    }

                    _result += space.PadRight(5) + string.Format("{0} {1},", colname, DataType) + Environment.NewLine;
                    //}



                }
                var dataPK = PropertiesTable.Where(w => w.default_value.Contains("nextval(")).FirstOrDefault();

                _result += !string.IsNullOrEmpty(dataPK?.column_name) ? space.PadRight(5) + string.Format("row_id integer,") + Environment.NewLine : "";
                _result += space.PadRight(5) + string.Format("lastupdatestamp integer,") + Environment.NewLine;
                //_result += space.PadRight(5) + string.Format("time_edit timestamp,") + Environment.NewLine;
                //_result += space.PadRight(5) + string.Format("user_input varchar,") + Environment.NewLine;
                //_result += space.PadRight(5) + string.Format("user_edit varchar,") + Environment.NewLine;
                _result = !string.IsNullOrEmpty(_result) ? _result.Remove(_result.LastIndexOf(',')) : _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
    }
}
