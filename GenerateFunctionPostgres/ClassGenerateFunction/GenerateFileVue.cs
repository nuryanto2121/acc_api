using GenerateFunctionPostgres.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;

namespace GenerateFunctionPostgres.ClassGenerateFunction
{
    public class GenerateFileVue
    {
        public string PathError;
        private string BodyTemplateFunction = string.Empty;
        private string TableName = string.Empty;
        private string ParameterInsert = string.Empty;
        private string space = string.Empty;
        private FunctionPostgres fnPostgres;
        public GenerateFileVue(string ConnectionString)
        {
            fnPostgres = new FunctionPostgres(ConnectionString);
        }
        public string CreateFileFunction(string OptionUrl, string FolderPath, string PathRoot)
        {
            string _result = string.Empty;
            try
            {
                string PathFile = Path.Combine(PathRoot, "Template", "TemplateVue.txt");
                using (StreamReader reader = new StreamReader(PathFile))
                {
                    BodyTemplateFunction = reader.ReadToEnd();
                }
                List<MOptionDB> OptionDbList = new List<MOptionDB>();
                OptionDbList = fnPostgres.OptionDBList(OptionUrl);
                if (OptionDbList.Count < 0)
                {
                    throw new Exception("Please Chek your Option URL");
                }
                TableName = OptionDbList.Find(m => m.line_no == 0).table_name;

                int iSave = OptionDbList.FindIndex(f => f.method_vue == "M_Save");
                if (iSave > 0)
                {

                }
                else
                {

                }
                int iUpdate = OptionDbList.FindIndex(f => f.method_vue == "M_Update");
                if (iUpdate > 0)
                {

                }
                else
                {

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _result;
        }
        private string MultiPost(List<MOptionDB> OptionDbList, string MethodAPi, string PathRoot)
        {
            string _result = string.Empty;
            try
            {
                string PathFile = Path.Combine(PathRoot, "Template", "MultiPostVue.txt");
                using (StreamReader reader = new StreamReader(PathFile))
                {
                    _result = reader.ReadToEnd();
                }
                string OrderBy = string.Empty;
                if (MethodAPi == "M_Save")
                {
                    OrderBy = "order_save";
                }
                else
                {
                    OrderBy = "order_update";
                }
                var propertyInfo = typeof(MOptionDB).GetProperty(OrderBy);
                var dataMultiList = OptionDbList.Where(w => w.method_vue.Contains(MethodAPi)).OrderBy(o => propertyInfo.GetValue(o, null)).ToList();
                // create variable 
                //for (int i = 0; i < dataMultiList.Count; i++)
                //{

                //}
                dataMultiList.ForEach(delegate (MOptionDB dt)
                {
                    var ParamFunction = fnPostgres.getParameterFunction(dt.sp);
                    ParamFunction.ForEach(delegate (ParameterPostgres dtPost)
                    {

                    });

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        private string SetParamPost(ParameterPostgres dataPostgres,string TableName)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string ComelTableName = textInfo.ToTitleCase(TableName.ToLower());
            string _result = string.Empty;
            string paramWihtValue = string.Empty;
            string param = dataPostgres.column_name.Remove(dataPostgres.column_name.IndexOf("p_"));
            string value = string.Empty;//"this."+ ComelTableName;

            switch (param)
            {
                case "ss_portfolio_id":
                    value = "this.getDataUser().portfolio_id";
                    break;
                case "ss_subportfolio_id":
                    value = "this.getDataUser().subportfolio_id";
                    break;
                case "user_input":
                case "user_id":
                case "user_edit":
                    value = "this.getDataUser().user_id";
                    break;
                case "lastupdatestamp":
                    value = "this.paramFromList.lastupdatestamp";
                    break;
                default:
                    value = "this." + ComelTableName + "." + param;
                    break;
            }
            paramWihtValue = string.Format("{0}:{1},", param, value);
            _result = space.PadRight(8) + paramWihtValue + Environment.NewLine;

            return _result;
        }
        private string ParamInsertUpdate(MOptionDB OptionSave)
        {
            string _result = string.Empty;
            try
            {
                var dataParam = fnPostgres.getParameterFunction(OptionSave.sp);
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                string ComelTableName = textInfo.ToTitleCase(OptionSave.table_name.ToLower());
                dataParam.ForEach(delegate (ParameterPostgres dt)
                {
                    //string paramWihtValue = string.Empty;
                    //string param = dt.column_name.Remove(dt.column_name.IndexOf("p_"));
                    //string value = string.Empty;//"this."+ ComelTableName;

                    //switch (param)
                    //{
                    //    case "ss_portfolio_id":
                    //        value = "this.getDataUser().portfolio_id";
                    //        break;
                    //    case "ss_subportfolio_id":
                    //        value = "this.getDataUser().subportfolio_id";
                    //        break;
                    //    case "user_input":
                    //    case "user_id":
                    //    case "user_edit":
                    //        value = "this.getDataUser().user_id";
                    //        break;
                    //    case "lastupdatestamp":
                    //        value = "this.paramFromList.lastupdatestamp";
                    //        break;
                    //    default:
                    //        value = "this." + ComelTableName + "." + param;
                    //        break;
                    //}
                    //paramWihtValue = string.Format("{0}:{1},", param, value);
                    _result += SetParamPost(dt, OptionSave.table_name);//space.PadRight(8) + paramWihtValue + Environment.NewLine;

                });
                _result = !string.IsNullOrEmpty(_result) ? _result.Remove(_result.LastIndexOf(",")) : _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        private string SetGetDataBy(MOptionDB OptionSave)
        {
            string _result = string.Empty;
            try
            {
                var dataParam = fnPostgres.GetResultFieldFunction(OptionSave.sp);
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                string ComelTableName = textInfo.ToTitleCase(OptionSave.table_name.ToLower());
                dataParam.ForEach(delegate (ParameterPostgres dt)
                {
                    string paramWihtValue = string.Empty;
                    string param = dt.column_name.Remove(dt.column_name.IndexOf("p_"));
                    string value = string.Empty;//"this."+ ComelTableName;

                    switch (param)
                    {
                        case "ss_portfolio_id":
                            value = "this.getDataUser().portfolio_id";
                            break;
                        case "ss_subportfolio_id":
                            value = "this.getDataUser().subportfolio_id";
                            break;
                        case "user_input":
                        case "user_id":
                        case "user_edit":
                            value = "this.getDataUser().user_id";
                            break;
                        case "lastupdatestamp":
                            value = "this.paramFromList.lastupdatestamp";
                            break;
                        default:
                            value = "this." + ComelTableName + "." + param;
                            break;
                    }
                    paramWihtValue = string.Format("{0}:{1},", param, value);
                    _result += space.PadRight(8) + paramWihtValue + Environment.NewLine;

                });
                _result = !string.IsNullOrEmpty(_result) ? _result.Remove(_result.LastIndexOf(",")) : _result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

    }
}
