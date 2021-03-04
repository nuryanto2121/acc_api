using Acc.Api.DataAccess;
using Acc.Api.Extenstion;
using Acc.Api.Helper;
using Acc.Api.Interface;
using Acc.Api.Models;
using Dapper;
using EncryptLibrary.AES256Encryption;
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
        private SsGroupRepo GroupRepo;
        private IFormulaEvaluator eval;
        private int ss_portfolio_id;
        private int ss_subportfolio_id;
        private string user_input;
        private string SpName;
        private Boolean isError;
        private readonly IEmailService _emailSender;
        public PortInService(IConfiguration Configuration, IEmailService EmailSender, IHostingEnvironment environment)
        {
            config = Configuration;
            connectionString = Tools.ConnectionString(Configuration);
            fn = new FunctionString(Tools.ConnectionString(Configuration));
            _environment = environment;
            OptionRepo = new OptionTemplateRepo(Configuration);
            GroupRepo = new SsGroupRepo(connectionString);
            _emailSender = EmailSender;
        }

        public async Task<Output> ReadDataExcelToDB(PortInFile portinFile)
        {
            //using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
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
                            eval = new HSSFFormulaEvaluator(hssfwb);
                        }
                        else
                        {
                            XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                            Settingsheet = hssfwb.GetSheet("Setting"); //get Setting sheet from workbook  
                            ImportSheet = hssfwb.GetSheet("Import"); //get Import sheet from workbook  
                            eval = new XSSFFormulaEvaluator(hssfwb);

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
                        JObject OutObj = new JObject();
                        List<JObject> ParamExcelList = new List<JObject>();

                        //if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)

                        int ix = 0;
                        int ie = 0;
                        conn.Open();
                        var trans = conn.BeginTransaction();
                        try
                        {

                            for (int iRow = (ImportSheet.FirstRowNum + 3); iRow <= ImportSheet.LastRowNum; iRow++) //Read Excel File
                            {
                                if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                                    conn.Open();

                                IRow row = ImportSheet.GetRow(iRow);
                                if (row == null) continue;
                                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                                DynamicParameters spParam = new DynamicParameters();
                                JObject ParamExcel = new JObject();
                                ParamPortIn dataPassing = new ParamPortIn();
                                Object affectedRows = null;
                                for (int iColumn = 0; iColumn <= ColumnsHeader.Count - 1; iColumn++)
                                {
                                    ICell cell = row.GetCell(iColumn);
                                    string fieldString = cell.GetFormattedCellValue(eval);
                                    //ParamExcel.Add(ColumnsHeader[iColumn], row.GetCell(iColumn).ToString());
                                    ParamExcel.Add(ColumnsHeader[iColumn], fieldString);
                                }
                                ListOutPut.Add(ParamExcel);
                                ix = iRow;
                                //try
                                //{

                                dataPassing.ss_portfolio_id = ss_portfolio_id;
                                dataPassing.ss_subportfolio_id = ss_subportfolio_id;
                                dataPassing.user_input = user_input;
                                dataPassing.data_port = ParamExcel;

                                spParam = fn.SpParameterPortIn(OptionRepo.getListParamType(SpName), dataPassing);

                                //affectedRows = conn.Query(SpName, spParam, commandType: CommandType.StoredProcedure, transaction: trans);
                                //affectedRows = await conn.QueryAsync(SpName, spParam, commandType: CommandType.StoredProcedure);
                                affectedRows = await conn.QueryAsync(SpName, spParam, commandType: CommandType.StoredProcedure, transaction: trans);
                                //}
                                ie += 1;



                            }


                            trans.Commit();
                            //scope.Complete();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            OutObj.Add("row", ix + 1);
                            OutObj.Add("message", ex.Message);
                            OutObj.Add("column", ParamExcelList[ie]);

                            ListOutPut.Add(OutObj);
                            //conn.BeginTransaction().Rollback();
                        }
                        finally
                        {
                            if (conn.State == ConnectionState.Open) conn.Close();
                        }


                        _result.Data = ListOutPut;
                    }
                }
                return _result;
            }
        }
        public async Task<Output> ReadDataExcelToDBNew(PortInFile portinFile)
        {
            //using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //using (IDbConnection conn = Tools.DBConnection(connectionString))
            //{
            #region set variable
            Output _result = new Output();
            ss_portfolio_id = Convert.ToInt32(fn.DecryptString(portinFile.ss_portfolio_id));
            ss_subportfolio_id = Convert.ToInt32(fn.DecryptString(portinFile.ss_subportfolio_id));
            user_input = fn.DecryptString(portinFile.user_input);
            IFormFile file = portinFile.file_portin;
            //string SpName = string.Empty;
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
                        eval = new HSSFFormulaEvaluator(hssfwb);
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        Settingsheet = hssfwb.GetSheet("Setting"); //get Setting sheet from workbook  
                        ImportSheet = hssfwb.GetSheet("Import"); //get Import sheet from workbook  
                        eval = new XSSFFormulaEvaluator(hssfwb);

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
                    if (SpName == "USER")
                    {
                        ListOutPut = await OutputPortinUser(ImportSheet, ColumnsHeader);
                    }
                    else
                    {
                        ListOutPut = await OutputPortin(ImportSheet, ColumnsHeader);
                    }


                    _result.Data = ListOutPut;
                }
            }
            if (isError == true)
            {
                _result.Error = isError;
                _result.Status = 500;
            }
            return _result;
            //}
        }

        public async Task<Output> ReadDataExcelToDBNewWithSetting(PortInFile portinFile)
        {
            //using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //using (IDbConnection conn = Tools.DBConnection(connectionString))
            //{
            #region set variable
            Output _result = new Output();
            ss_portfolio_id = Convert.ToInt32(fn.DecryptString(portinFile.ss_portfolio_id));
            ss_subportfolio_id = Convert.ToInt32(fn.DecryptString(portinFile.ss_subportfolio_id));
            user_input = fn.DecryptString(portinFile.user_input);
            IFormFile file = portinFile.file_portin;
            //string SpName = string.Empty;
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
                        //Settingsheet = hssfwb.GetSheet("Setting"); //get Setting sheet from workbook  
                        ImportSheet = hssfwb.GetSheet("Import"); //get Import sheet from workbook  
                        eval = new HSSFFormulaEvaluator(hssfwb);
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        //Settingsheet = hssfwb.GetSheet("Setting"); //get Setting sheet from workbook  
                        ImportSheet = hssfwb.GetSheet("Import"); //get Import sheet from workbook  
                        eval = new XSSFFormulaEvaluator(hssfwb);

                    }
                    #endregion

                    #region Read Storeprocedure
                    string fileN = fileName.Replace(sFileExtension, "");
                    TablePortinFunction dt = OptionRepo.GetdataTableFunction(fileN);
                    if (dt == null)
                    {
                        throw new Exception("Table Setting Function null.");
                    }
                    SpName = dt.function_name;
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
                    if (SpName == "USER")
                    {
                        ListOutPut = await OutputPortinUser(ImportSheet, ColumnsHeader);
                    }
                    else
                    {
                        ListOutPut = await OutputPortin(ImportSheet, ColumnsHeader);
                    }


                    _result.Data = ListOutPut;
                }
            }
            if (isError == true)
            {
                _result.Error = isError;
                _result.Status = 500;
            }
            return _result;
            //}
        }
        private async Task<List<JObject>> OutputPortin(ISheet ImportSheet, StringCollection ColumnsHeader)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                List<JObject> ListOutPut = new List<JObject>();
                JObject OutObj = new JObject();
                List<JObject> ParamExcelList = new List<JObject>();

                //if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)

                int ix = 0;
                int ie = 0;
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {

                    for (int iRow = (ImportSheet.FirstRowNum + 3); iRow <= ImportSheet.LastRowNum; iRow++) //Read Excel File
                    {
                        if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                            conn.Open();

                        IRow row = ImportSheet.GetRow(iRow);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        DynamicParameters spParam = new DynamicParameters();
                        JObject ParamExcel = new JObject();
                        ParamPortIn dataPassing = new ParamPortIn();
                        Object affectedRows = null;
                        for (int iColumn = 0; iColumn <= ColumnsHeader.Count - 1; iColumn++)
                        {
                            ICell cell = row.GetCell(iColumn);
                            string fieldString = cell.GetFormattedCellValue(eval);
                            //ParamExcel.Add(ColumnsHeader[iColumn], row.GetCell(iColumn).ToString());
                            ParamExcel.Add(ColumnsHeader[iColumn], fieldString);
                        }
                        ParamExcelList.Add(ParamExcel);
                        ix = iRow;

                        dataPassing.ss_portfolio_id = ss_portfolio_id;
                        dataPassing.ss_subportfolio_id = ss_subportfolio_id;
                        dataPassing.user_input = user_input;
                        dataPassing.data_port = ParamExcel;

                        spParam = fn.SpParameterPortIn(OptionRepo.getListParamType(SpName), dataPassing);
                        affectedRows = await conn.QueryAsync(SpName, spParam, commandType: CommandType.StoredProcedure, transaction: trans);

                        ie += 1;

                    }


                    trans.Commit();
                }
                catch (Exception ex)
                {
                    isError = true;
                    trans.Rollback();
                    OutObj.Add("row", ix + 1);
                    OutObj.Add("message", ex.Message);
                    OutObj.Add("column", ParamExcelList[ie]);

                    ListOutPut.Add(OutObj);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
                return ListOutPut;
            }


        }

        private async Task<List<JObject>> OutputPortinUser(ISheet ImportSheet, StringCollection ColumnsHeader)
        {
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                List<JObject> ListOutPut = new List<JObject>();
                JObject OutObj = new JObject();
                List<JObject> ParamExcelList = new List<JObject>();

                //if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)

                int ix = 0;
                int ie = 0;
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {

                    for (int iRow = (ImportSheet.FirstRowNum + 3); iRow <= ImportSheet.LastRowNum; iRow++) //Read Excel File
                    {
                        if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                            conn.Open();

                        IRow row = ImportSheet.GetRow(iRow);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        DynamicParameters spParam = new DynamicParameters();
                        JObject ParamExcel = new JObject();
                        ParamPortIn dataPassing = new ParamPortIn();
                        SsUser dataUser = new SsUser();
                        for (int iColumn = 0; iColumn <= ColumnsHeader.Count - 1; iColumn++)
                        {
                            ICell cell = row.GetCell(iColumn);
                            string fieldString = cell.GetFormattedCellValue(eval);
                            //ParamExcel.Add(ColumnsHeader[iColumn], row.GetCell(iColumn).ToString());
                            ParamExcel.Add(ColumnsHeader[iColumn], fieldString);
                        }
                        ParamExcelList.Add(ParamExcel);
                        ix = iRow;

                        string GroupCd = ParamExcel["group_cd"].ToString();
                        string ParamWHere = string.Format("short_descs = '{0}' AND ss_portfolio_id = {1}", GroupCd, ss_portfolio_id);
                        int GroupID = Convert.ToInt32(GroupRepo.SelectScalar(Enum.SQL.Function.Aggregate.Min, "ss_group_id", ParamWHere));
                        if (GroupID == 0)
                        {
                            throw new Exception("User Group Not Found.");
                        }
                        dataUser.portfolio_id = ss_portfolio_id;
                        dataUser.subportfolio_id = ss_subportfolio_id;
                        dataUser.user_input = user_input;
                        dataUser.user_edit = user_input;
                        dataUser.user_id = ParamExcel["user_id"].ToString();
                        dataUser.user_name = ParamExcel["user_name"].ToString();
                        dataUser.email = ParamExcel["email"].ToString();
                        dataUser.hand_phone = ParamExcel["handphone"].ToString();
                        dataUser.address = ParamExcel["address"].ToString();
                        dataUser.user_level = "S"; dataUser.is_inactive = "B";
                        dataUser.password = EncryptionLibrary.EncryptText("1");
                        dataUser.group_id = GroupID;

                        SaveUser(conn, trans, dataUser);

                        ie += 1;

                        // Send Email

                    }


                    trans.Commit();
                }
                catch (Exception ex)
                {
                    isError = true;
                    trans.Rollback();
                    OutObj.Add("row", ix + 1);
                    OutObj.Add("message", ex.Message);
                    OutObj.Add("column", ParamExcelList[ie]);

                    ListOutPut.Add(OutObj);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
                return ListOutPut;
            }


        }

        public async Task<Output> ReadDataExcelToDBUser(PortInFile portinFile)
        {
            //using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (IDbConnection conn = Tools.DBConnection(connectionString))
            {
                #region set variable
                Output _result = new Output();
                int ss_portfolio_id = Convert.ToInt32(fn.DecryptString(portinFile.ss_portfolio_id));
                int ss_subportfolio_id = Convert.ToInt32(fn.DecryptString(portinFile.ss_subportfolio_id));
                string user_input = fn.DecryptString(portinFile.user_input);
                IFormFile file = portinFile.file_portin;
                //string SpName = string.Empty;
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
                        IFormulaEvaluator eval;
                        #region Read Extention Driver
                        if (sFileExtension == ".xls")
                        {
                            HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                            Settingsheet = hssfwb.GetSheet("Setting"); //get Setting sheet from workbook  
                            ImportSheet = hssfwb.GetSheet("Import"); //get Import sheet from workbook  
                            eval = new HSSFFormulaEvaluator(hssfwb);
                        }
                        else
                        {
                            XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                            Settingsheet = hssfwb.GetSheet("Setting"); //get Setting sheet from workbook  
                            ImportSheet = hssfwb.GetSheet("Import"); //get Import sheet from workbook  
                            eval = new XSSFFormulaEvaluator(hssfwb);

                        }
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
                        JObject OutObj = new JObject();
                        List<JObject> ParamExcelList = new List<JObject>();

                        //if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)

                        int ix = 0;
                        int ie = 0;
                        conn.Open();
                        var trans = conn.BeginTransaction();
                        try
                        {


                            for (int iRow = (ImportSheet.FirstRowNum + 3); iRow <= ImportSheet.LastRowNum; iRow++) //Read Excel File
                            {
                                if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                                    conn.Open();

                                IRow row = ImportSheet.GetRow(iRow);
                                if (row == null) continue;
                                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                                DynamicParameters spParam = new DynamicParameters();
                                JObject ParamExcel = new JObject();
                                //ParamPortIn dataPassing = new ParamPortIn();
                                SsUser dataUser = new SsUser();
                                for (int iColumn = 0; iColumn <= ColumnsHeader.Count - 1; iColumn++)
                                {
                                    ICell cell = row.GetCell(iColumn);
                                    string fieldString = cell.GetFormattedCellValue(eval);
                                    //ParamExcel.Add(ColumnsHeader[iColumn], row.GetCell(iColumn).ToString());
                                    ParamExcel.Add(ColumnsHeader[iColumn], fieldString);
                                }
                                ParamExcelList.Add(ParamExcel);
                                ix = iRow;
                                //try
                                //{

                                //dataPassing.ss_portfolio_id = ss_portfolio_id;
                                //dataPassing.ss_subportfolio_id = ss_subportfolio_id;
                                //dataPassing.user_input = user_input;
                                //dataPassing.data_port = ParamExcel;
                                string GroupCd = ParamExcel["group_cd"].ToString();
                                string ParamWHere = string.Format("short_descs = '{0}' AND ss_portfolio_id = {1}", GroupCd, ss_portfolio_id);
                                int GroupID = Convert.ToInt32(GroupRepo.SelectScalar(Enum.SQL.Function.Aggregate.Min, "ss_group_id", ParamWHere));
                                if (GroupID == 0)
                                {
                                    throw new Exception("User Group Not Found.");
                                }
                                dataUser.portfolio_id = ss_portfolio_id;
                                dataUser.subportfolio_id = ss_subportfolio_id;
                                dataUser.user_input = user_input;
                                dataUser.user_edit = user_input;
                                dataUser.user_id = ParamExcel["user_id"].ToString();
                                dataUser.user_name = ParamExcel["user_name"].ToString();
                                dataUser.email = ParamExcel["email"].ToString();
                                dataUser.hand_phone = ParamExcel["handphone"].ToString();
                                dataUser.address = ParamExcel["address"].ToString();
                                dataUser.user_level = "S"; dataUser.is_inactive = "B";
                                dataUser.password = EncryptionLibrary.EncryptText("1");
                                dataUser.group_id = GroupID;

                                SaveUser(conn, trans, dataUser);
                                //spParam = fn.SpParameterPortIn(OptionRepo.getListParamType(SpName), dataPassing);

                                //affectedRows = conn.Query(SpName, spParam, commandType: CommandType.StoredProcedure, transaction: trans);
                                //affectedRows = await conn.QueryAsync(SpName, spParam, commandType: CommandType.StoredProcedure);
                                //affectedRows = await conn.QueryAsync(SpName, spParam, commandType: CommandType.StoredProcedure, transaction: trans);
                                //}
                                ie += 1;



                            }

                            trans.Commit();
                            //scope.Complete();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            OutObj.Add("row", ix + 1);
                            OutObj.Add("message", ex.Message);
                            OutObj.Add("column", ParamExcelList[ie]);

                            ListOutPut.Add(OutObj);
                            //conn.BeginTransaction().Rollback();
                        }
                        finally
                        {
                            if (conn.State == ConnectionState.Open) conn.Close();
                        }


                        _result.Data = ListOutPut;
                    }
                }
                return _result;
            }
        }

        void SaveUser(IDbConnection Conn, IDbTransaction Trans, SsUser domain)
        {
            try
            {
                string sqlQuery = @"INSERT INTO public.ss_user (
                                        user_id,                ss_group_id,
                                        user_name,              password,
                                        email,                  user_level,
                                        expired_date,           is_inactive,
                                        job_title,              hand_phone,
                                        last_change_password,   default_language,
                                        user_input,             user_edit,
                                        portfolio_id,           subportfolio_id,
                                        time_input,             time_edit,
                                        file_name,              path_file,
                                        address,                notes
                                )
                                VALUES (
                                      @user_id,                                      @group_id,
                                      @user_name,                                      @password,
                                      @email,                                      @user_level,
                                      @expired_date,                                      @is_inactive,
                                      @job_title,                                      @hand_phone,
                                      @last_change_password,                                      @default_language,
                                      @user_input,                                      @user_edit,
                                      @portfolio_id,                                      @subportfolio_id,
                                      @time_input,                                      @time_edit,
                                      @file_name,                                      @path_file,
                                      @address,                                         @notes
                                );";

                Conn.Execute(sqlQuery, domain, transaction: Trans);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task SendEmailForgotAsync(SsUser dataUser, string OTP)
        {
            try
            {
                string BodyTemplateFunction = string.Empty;
                if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
                {
                    _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }
                string PathFile = Path.Combine(_environment.WebRootPath, "Template", "AccessPasswordUser.html");

                //if (dataHeader.form_type.ToUpper() == "FORM")
                //{
                //    PathFile = Path.Combine(PathRoot, "Template", "MasterCrudPostgresForm.txt");
                //}
                using (StreamReader reader = new StreamReader(PathFile))
                {
                    BodyTemplateFunction = reader.ReadToEnd();
                }
                BodyTemplateFunction = BodyTemplateFunction.Replace("{UserName}", dataUser.user_name);
                BodyTemplateFunction = BodyTemplateFunction.Replace("{OTP}", OTP);

                EmailModel ModelEmail = new EmailModel();
                ModelEmail.to = dataUser.email;
                ModelEmail.subject = "Access User";
                ModelEmail.body = BodyTemplateFunction;

                ModelEmail.user_id = EncryptionLibrary.EncryptText(dataUser.user_id);

                Output _result = await _emailSender.SendEmailAsync(ModelEmail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
