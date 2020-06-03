using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Acc.Api.Authorize;
using Acc.Api.Helper;
using Acc.Api.Models;
using Acc.Api.Services;
using ClosedXML.Excel;
using GenerateFunctionPostgres.ClassGenerateFunction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SelectPdf;

namespace Acc.Api.Controllers.FileUploadImport
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private IHostingEnvironment _environment;
        private FunctionString fn;
        private FileService FileService;
        private ProsessGenerateFunction ProsesGenerateFunction;
        public FileController(IConfiguration configuration, IHostingEnvironment environment)
        {
            _environment = environment;
            fn = new FunctionString(Tools.ConnectionString(configuration));
            FileService = new FileService(configuration, environment);
            ProsesGenerateFunction = new ProsessGenerateFunction(Tools.ConnectionString(configuration));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        [HttpPost("UploadFile")]
        [APIAuthorizeAttribute]
        [ProducesResponseType(typeof(Output), 200)]
        //public void UploadFile([FromForm]UploadFile uploadFile)
        public async Task<Output> UploadFile([FromForm]UploadFile uploadFile)
        {
            var po = new Output();
            try
            {
                var folderName = Path.Combine("FileUpload", uploadFile.modulecd);
                var pathToSave = Path.Combine(_environment.WebRootPath, folderName);
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                Dictionary<string, string> data = new Dictionary<string, string>();
                //foreach (var file in uploadFile.images)
                //{
                var file = uploadFile.images;
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                string fileType = Path.GetExtension(fileName).ToLower();
                var nameunix = DateTime.Now.Ticks.ToString() + fileType;
                //var fullPath = Path.Combine(pathToSave, linkAttachment.ToString());
                var fullPath = Path.Combine(pathToSave, nameunix);
                var dbPath = Path.Combine(folderName, nameunix);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                data.Add("path", dbPath);
                data.Add("name", file.FileName);


                //}
                po.Data = data;
                //string pModuleCd = !string.IsNullOrEmpty(Request.Form["ModuleCd"]) ? Request.Form["ModuleCd"].ToString() : "";
                //string pSubportfolioCd = !string.IsNullOrEmpty(Request.Form["SubPortfolio"]) ? Request.Form["SubPortfolio"].ToString() : "";
                //string pDelimiter = !string.IsNullOrEmpty(Request.Form["Delimiter"]) ? Request.Form["Delimiter"].ToString() : "";
                //string pPath = !string.IsNullOrEmpty(Request.Form["Path"]) ? fn.PathString(Request.Form["Path"].ToString()) : "";

                //string pathToSave2 = !string.IsNullOrEmpty(pModuleCd) ? Path.Combine("FileUpload", pSubportfolioCd, pModuleCd) : Path.Combine("Picture", pSubportfolioCd, "Uploads");
                //string pathToSave = !string.IsNullOrEmpty(pPath) ? pPath : pathToSave2;
                //if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
                //{
                //    _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                //}
                //pathToSave = Path.Combine(_environment.WebRootPath, pathToSave);
                ////pathToSave = _environment.WebRootPath + pathToSave;

                //if (!Directory.Exists(pathToSave))
                //{
                //    Directory.CreateDirectory(pathToSave);
                //}

                //var docfiles = new List<string>();
                //foreach (var file in Request.Form.Files)
                //{

                //    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                //    string fileType = Path.GetExtension(fileName).ToLower();
                //    string PathSaveWithName = Path.Combine(pathToSave, fileName);
                //    if (System.IO.File.Exists(PathSaveWithName))
                //    {
                //        fileName = string.Format("{0}_{1}{2}", fileName.Replace(fileType, ""), DateTime.Now.ToString("ddMMyyyyhhmmss"), fileType);
                //    }

                //    using (var stream = new FileStream(Path.Combine(pathToSave, fileName), FileMode.Create))
                //    {
                //        await file.CopyToAsync(stream);
                //    }

                //    docfiles.Add(fileName);
                //}

                //po.Data = docfiles;
            }
            catch (Exception ex)
            {
                po.Error = true;
                po.Message = ex.Message;
            }

            return po;

        }

        /// <summary>
        /// Generate File excel,pdf dan csv dari list
        /// </summary>
        /// <param name="N">FileName</param>
        /// <param name="T">Type File : 1 = excel, 2 = pdf ,3 = csv</param>
        /// <param name="K">Key(Token) dari list</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested file</response>
        [AllowAnonymous]
        [HttpGet("ExportFileList")]
        public async Task<IActionResult> ExportFileList(string N, string T, string K)
        {
            string fType = string.Empty;
            switch (T)
            {
                case "1":
                    fType = ".xlsx";
                    break;
                case "2":
                    fType = ".pdf";
                    break;
                case "3":
                    fType = ".csv";
                    break;
            }
            var fileNames = N + DateTime.Now.ToString("ddMMyyyyhhmm");

            var stream = new MemoryStream();
            try
            {
                if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
                {
                    _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }
                string pathToSave = Path.Combine(_environment.WebRootPath, "temp");
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                var workbookBytes = new byte[0];
                var dd = FileService.GetDataExport(K);//new DataTable();
                if (T == "1")
                {
                    string fileTosaveExcel = Path.Combine(pathToSave, fileNames + ".xlsx");
                    //string fileTosavePdf = Path.Combine(pathToSave, fileNames + ".pdf");

                    fileNames += fType;



                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var iRange = dd.Columns.Count == 0 ? 5 : dd.Columns.Count;

                        wb.Style.Font.FontName = "Arial";
                        wb.Style.Font.FontSize = 9;
                        var ws = wb.Worksheets.Add("sheet1");
                        ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                        ws.PageSetup.FitToPages(2, 2);
                        //ws.Cells().Clear();
                        ws.Cell(1, 1).Value = N;

                        ws.Range(1, 1, 1, iRange).Merge().AddToNamed("Titles");

                        var tableWithData = ws.Cell(3, 1).InsertTable(dd);

                        var titlesStyle = wb.Style;
                        titlesStyle.Font.Bold = true;
                        titlesStyle.Font.FontSize = 14;

                        titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;

                        ws.Columns().AdjustToContents();

                        using (var ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            //if (T == "2")
                            //{
                            //    wb.SaveAs(fileTosaveExcel);
                            //    FileService.ConvertToPdf(fileTosaveExcel);

                            //    //var pdfStream = new FileStream(fileTosavePdf, FileMode.Open);
                            //    using (var pdfStream = new FileStream(fileTosavePdf, FileMode.Open))
                            //    {
                            //        await pdfStream.CopyToAsync(stream);
                            //    }
                            //}
                            //else
                            //{
                            workbookBytes = ms.ToArray();



                        }

                    }

                    if (System.IO.File.Exists(fileTosaveExcel))
                    {
                        System.IO.File.Delete(fileTosaveExcel);
                    }
                    //if (System.IO.File.Exists(fileTosavePdf))
                    //{
                    //    System.IO.File.Delete(fileTosavePdf);
                    //}

                }
                else if (T == "2")
                {
                    string fileTosavePdf = Path.Combine(pathToSave, fileNames + fType);
                    string fileTosaveHtml = Path.Combine(pathToSave, fileNames + ".html");

                    fileNames += fType;

                    string BodyHTML = string.Empty;
                    FileService.GenPDF(N, dd, fileTosaveHtml);
                    using (StreamReader reader = new StreamReader(fileTosaveHtml))
                    {
                        BodyHTML = reader.ReadToEnd();
                    }

                    HtmlToPdf converter = new HtmlToPdf();
                    converter.Options.PdfPageSize = PdfPageSize.A4;
                    converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                    converter.Options.WebPageWidth = 1024;
                    converter.Options.WebPageHeight = 0;
                    converter.Options.CustomCSS = @"th {
	                                                    color: white;
	                                                    background-color: #4438ff;
                                                    }

                                                    td, th {
                                                      border: 1px solid #dddddd;
                                                      text-align: left;
                                                      padding: 8px;
                                                    }

                                                    tr:nth-child(even) {
                                                      background-color: #4438ff57;
                                                    }";
                    PdfDocument doc = converter.ConvertHtmlString(BodyHTML);


                    // save pdf document
                    //doc.Save(Response, false, "Sample.pdf");
                    doc.Save(fileTosavePdf);

                    // close pdf document
                    doc.Close();
                    //var pdfStream = new FileStream(fileTosavePdf, FileMode.Open);
                    using (var pdfStream = new FileStream(fileTosavePdf, FileMode.Open))
                    {
                        await pdfStream.CopyToAsync(stream);
                    }

                    if (System.IO.File.Exists(fileTosavePdf))
                    {
                        System.IO.File.Delete(fileTosavePdf);
                    }
                    if (System.IO.File.Exists(fileTosaveHtml))
                    {
                        System.IO.File.Delete(fileTosaveHtml);
                    }

                }
                else // csv
                {
                    string fileTosave = Path.Combine(pathToSave, fileNames + fType);
                    fileNames += fType;
                    FileService.ToCSV(dd, fileTosave);

                    using (var pdfStream = new FileStream(fileTosave, FileMode.Open))
                    {
                        await pdfStream.CopyToAsync(stream);
                    }

                    if (System.IO.File.Exists(fileTosave))
                    {
                        System.IO.File.Delete(fileTosave);
                    }
                }

                if (T == "1")
                {
                    stream = new MemoryStream(workbookBytes);
                }

                stream.Position = 0;


            }
            catch (Exception ex)
            {
                //FileService.textError(ex);
                Tools.TextError(_environment, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return File(stream, "application/octet-stream", fileNames); // returns a FileStreamResult
        }

        /// <summary>
        /// untuk generate token baru berdasarkan row yg dipilih
        /// </summary>
        /// <param name="Jmodel"></param>
        /// <returns></returns>
        [HttpPost("GetTokenExport")]
        [APIAuthorizeAttribute]
        public IActionResult GetTokenExport([FromBody]ExportExcelJson Jmodel)
        {
            Output result = new Output();
            try
            {
                result = FileService.GenerateTokenExcel(Jmodel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Tools.Error(ex.Message));
            }

            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GenerateFileFunctionPostgresWithTable(string TableName)
        {
            var stream = new MemoryStream();
            string fileNames = string.Empty;//string.Format("CRUD_{0}.sql", TableName);
            try
            {
                if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
                {
                    _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }

                string pathToSave = Path.Combine(_environment.WebRootPath, "FolderFunctionPostgres");

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                fileNames = ProsesGenerateFunction.CreateFileFunction(TableName, pathToSave, _environment.WebRootPath);

                string PathFile = Path.Combine(pathToSave, fileNames);
                var workbookBytes = new byte[0];
                System.IO.FileStream fs = new System.IO.FileStream(PathFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                fs.CopyTo(stream);
                stream.Position = 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return File(stream, "application/octet-stream", fileNames);
        }

    }
}