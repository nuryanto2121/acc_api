using Acc.Api.Helper;
using Acc.Api.Models;
using EncryptLibrary.AES256Encryption;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class FileService
    {
        IConfiguration config;
        private IHostingEnvironment _environment;
        private FunctionString fn;
        private DynamicService dn;
        public FileService(IConfiguration Configuration, IHostingEnvironment environment)
        {
            config = Configuration;
            fn = new FunctionString(Tools.ConnectionString(Configuration));
            dn = new DynamicService(Configuration);
            config = Configuration;
            _environment = environment;
        }
        public DataTable GetDataExport(string encodeString)
        {
            DataTable _result = new DataTable();
            try
            {

                encodeString = encodeString.Replace(" ", "+");
                if (!string.IsNullOrEmpty(encodeString))
                {
                    var Key = EncryptionLibrary.DecryptText(encodeString);
                    string[] parts = Key.Split(new string[] { ":" }, StringSplitOptions.None);

                    string TableName = parts[0];    //table
                    string StringParam = parts[1];  // Where
                    string FieldColumn = parts[2];  // Field select
                    string OrderBy = parts[3];  // Field select

                    FieldColumn = FieldColumn.ToUpper();
                    string[] FieldColumnArr = FieldColumn.Split(',').Where(val => val != "NO").ToArray();
                    FieldColumn = string.Join(",", FieldColumnArr);
                    //StringParam = if (StringParam;
                    if (StringParam == "X") StringParam = "";

                    StringParam = StringParam.ToUpper().IndexOf("WHERE") == 0 ? StringParam.Remove(StringParam.ToUpper().IndexOf("WHERE"), 5) : StringParam;

                    var datax = dn.QueryList(TableName, StringParam, FieldColumn);

                    _result = fn.ToDataTable(datax);
                }

                //return _result;


            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return _result;
        }

        public Output GenerateTokenExcel(ExportExcelJson JObj)
        {
            Output token = new Output();

            try
            {
                var Key = EncryptionLibrary.DecryptText(JObj.Token);
                string[] parts = Key.Split(new string[] { ":" }, StringSplitOptions.None);

                string TableName = parts[0];    //table
                string StringParam = "";// parts[1];  // Where
                string FieldColumn = parts[2];  // Field select
                string SortField = parts[3];  // Sort select

                if (string.IsNullOrEmpty(JObj.Data))
                {
                    token.Error = true;
                    token.Message = "Data Not Valid.";
                    return token;
                }

                StringParam = string.Format("Row_Id in ({0})", JObj.Data);
                var Encript = string.Join(":", new string[] {
                        //OptionSeq.ToString(),
                        //LineNo.ToString(),
                        TableName,
                        StringParam,
                        FieldColumn,
                        SortField

                    });


                token.Data = JObject.Parse("{'Token':'" + EncryptionLibrary.EncryptText(Encript) + "'}");//;

            }
            catch (Exception ex)
            {
                //fn.InsertErrorLog(Global.UserId, "File : " + ex.StackTrace);
                token.Error = true;
                token.Message = ex.Message;
            }
            return token;

        }

        public static void ConvertToPdf(string excelFilePath)
        {
            using (var excelApplication = new ExcelApplicationWrapper())
            {
                try
                {
                    //var thisFileWorkbook = excelApplication.ExcelApplication.Workbooks.Open(excelFilePath);

                    //string newPdfFilePath = Path.Combine(
                    //    Path.GetDirectoryName(excelFilePath),
                    //    $"{Path.GetFileNameWithoutExtension(excelFilePath)}.pdf");

                    //((Microsoft.Office.Interop.Excel._Worksheet)
                    //        thisFileWorkbook.ActiveSheet).PageSetup.Orientation =
                    //        Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape;

                    //thisFileWorkbook.ExportAsFixedFormat(
                    //    Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF,
                    //    newPdfFilePath);

                    //thisFileWorkbook.Close(false, excelFilePath);
                    //Marshal.ReleaseComObject(thisFileWorkbook);

                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }


        public void ToCSV(DataTable dtDataTable, string strFilePath)
        {
            try
            {
                string delimiter = config.GetValue<string>("appSetting:Delimeter");

                StreamWriter sw = new StreamWriter(strFilePath, false);
                //headers  
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    sw.Write(dtDataTable.Columns[i]);
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(delimiter);
                    }
                }
                sw.Write(sw.NewLine);
                foreach (DataRow dr in dtDataTable.Rows)
                {
                    for (int i = 0; i < dtDataTable.Columns.Count; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            string value = dr[i].ToString();
                            if (value.Contains(delimiter))
                            {
                                value = String.Format("\"{0}\"", value);
                                sw.Write(value);
                            }
                            else
                            {
                                sw.Write(dr[i].ToString());
                            }
                        }
                        if (i < dtDataTable.Columns.Count - 1)
                        {
                            sw.Write(delimiter);
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        public void textError(Exception error)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
                {
                    _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }
                string pathToSave = Path.Combine(_environment.WebRootPath, "error");
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                string strFilePath = Path.Combine(pathToSave, string.Format("{0}.txt", DateTime.Now.ToString("ddMMyyyy")));
                StreamWriter sw = new StreamWriter(strFilePath, false);

                sw.Write(error.Message);
                sw.Write(sw.NewLine);
                sw.Write(error.StackTrace);

                sw.Close();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public void GenPDF(string Judul, DataTable dtDataTable, string strFilePath)
        {
            try
            {
                string BodyHTML = string.Empty;
                string space = string.Empty;
                if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
                {
                    _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }
                string PathFile = Path.Combine(_environment.WebRootPath, "template", "datatable_pdf.html");
                using (StreamReader reader = new StreamReader(PathFile))
                {
                    BodyHTML = reader.ReadToEnd();
                }
                BodyHTML = BodyHTML.Replace("{Judul}", Judul);

                //Header
                string sHeader = string.Empty;
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    string columsName = dtDataTable.Columns[i].ToString();
                    sHeader += space.PadRight(4) + string.Format("<th>{0}</th>", columsName) + Environment.NewLine;
                }

                BodyHTML = BodyHTML.Replace("{Header}", sHeader);

                // Detail
                string sDetail = string.Empty;
                foreach (DataRow dr in dtDataTable.Rows)
                {
                    sDetail += space.PadRight(0) + "<tr>" + Environment.NewLine;
                    for (int i = 0; i < dtDataTable.Columns.Count; i++)
                    {

                        string value = dr[i].ToString();
                        sDetail += space.PadRight(4) + string.Format("<td>{0}</td>", value) + Environment.NewLine;


                    }
                    sDetail += space.PadRight(0) + "</tr>" + Environment.NewLine;
                }

                BodyHTML = BodyHTML.Replace("{Detail}", sDetail);

                string[] sBodyHtml = BodyHTML.Split(
                                   new[] { Environment.NewLine },
                                   StringSplitOptions.None
                               );

                Tools.writeFile(sBodyHtml, strFilePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
