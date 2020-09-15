using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Models;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Acc.Api.Services
{
    public class PortInService
    {
        IConfiguration config;
        private IHostingEnvironment _environment;
        private FunctionString fn;
        private string connectionString;
        private OptionTemplateRepo OptionRepo;
        public PortInService(IConfiguration Configuration, IHostingEnvironment environment)
        {
            config = Configuration;
            connectionString = Tools.ConnectionString(Configuration);
            fn = new FunctionString(Tools.ConnectionString(Configuration));
            _environment = environment;
            OptionRepo = new OptionTemplateRepo(Configuration);
        }

        public async Task<Output> ReadDataExcelToDB(PortInFile portinFile)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                #region set variable
                Output _result = new Output();
                int ss_portfolio_id = Convert.ToInt32(fn.DecryptString(portinFile.ss_portfolio_id));
                int ss_subportfolio_id = Convert.ToInt32(fn.DecryptString(portinFile.ss_subportfolio_id));
                string user_input = fn.DecryptString(portinFile.user_input);
                IFormFile file = portinFile.file_portin;
                string SpName = string.Empty;
                StringCollection ColumnsHeader = new StringCollection();            
                var stream = new MemoryStream();
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                ISheet Settingsheet, ImportSheet;
                #endregion
                if (file.Length > 0)
                {
                    
                    using (var ms = new MemoryStream())
                    {
                        //file.CopyTo(ms);
                        await file.CopyToAsync(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);
                        byte[] bytes = Convert.FromBase64String(s);
                        stream = new MemoryStream(bytes);

                        List<JObject> ListOutPut = new List<JObject>();

                        #region Read Extention Driver
                        if (sFileExtension == ".xls")
                        {
                            HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                            Settingsheet = hssfwb.GetSheet("Setting"); //get Setting sheet from workbook  
                            ImportSheet = hssfwb.GetSheet("Import"); //get Import sheet from workbook  
                        }
                        else
                        {
                            XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                            Settingsheet = hssfwb.GetSheet("Setting"); //get Setting sheet from workbook  
                            ImportSheet = hssfwb.GetSheet("Import"); //get Import sheet from workbook  

                        }
                        #endregion

                        #region Read Storeprocedure
                        //Get SP Name in Setting Sheet
                        var cr = new CellReference("B1");
                        var rows = Settingsheet.GetRow(cr.Row);
                        var cells = rows.GetCell(cr.Col);
                        SpName = cells.StringCellValue;
                        #endregion

                        #region Set Header 
                        IRow headerRow = ImportSheet.GetRow(0); //Get Header Row
                                                                //IRow headerRow = ImportSheet.G; //Get Header Row
                        int cellCount = headerRow.LastCellNum;
                        //sb.Append("<table class='table table-bordered'><tr>");
                        for (int j = 0; j < cellCount; j++)
                        {
                            NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
                            if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                            //sb.Append("<th>" + cell.ToString() + "</th>");
                            ColumnsHeader.Add(cell.ToString());
                        }
                        #endregion

                        //if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                        conn.Open();

                        var trans = conn.BeginTransaction();

                        for (int iRow = (ImportSheet.FirstRowNum + 3); iRow <= ImportSheet.LastRowNum; iRow++) //Read Excel File
                        {
                            if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                                conn.Open();

                            JObject OutObj = new JObject();

                            IRow row = ImportSheet.GetRow(iRow);
                            if (row == null) continue;
                            if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                            DynamicParameters spParam = new DynamicParameters();
                            JObject ParamExcel = new JObject();
                            ParamPortIn dataPassing = new ParamPortIn();
                            Object affectedRows = null;
                            for (int iColumn = 0; iColumn <= ColumnsHeader.Count - 1; iColumn++)
                            {
                                NPOI.SS.UserModel.ICell cell = row.GetCell(iColumn);
                                ParamExcel.Add(ColumnsHeader[iColumn], row.GetCell(iColumn).ToString());
                            }

                            try
                            {

                                dataPassing.ss_portfolio_id = ss_portfolio_id;
                                dataPassing.ss_subportfolio_id = ss_subportfolio_id;
                                dataPassing.user_input = user_input;
                                dataPassing.data_port = ParamExcel;

                                spParam = fn.SpParameterPortIn(OptionRepo.getListParamType(SpName), dataPassing);

                                affectedRows = conn.Query(SpName, spParam, commandType: CommandType.StoredProcedure, transaction: trans);
                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                OutObj.Add("row", iRow + 1);
                                OutObj.Add("message", ex.Message);
                                OutObj.Add("column", ParamExcel);

                                ListOutPut.Add(OutObj);
                            }
                            finally
                            {
                                if (conn.State == ConnectionState.Open) conn.Close();
                            }

                        }

                        trans.Commit();

                        _result.Data = ListOutPut;
                    }
                }
                return _result;
            }
        }
    }
}
