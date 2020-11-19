using Microsoft.AspNetCore.Hosting;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public static class HtmlToPdfOtn
    {
        public static IHostingEnvironment _environment { get; set; }
        public static string HTML;
        public static string Subject;
        public static string PathSave { get; set; }
        public static string DBpath { get; set; }
        public static bool IsReplace { get; set; } = false;
        public static string FileName { get; set; }


        //public static string PathPDF(IHostingEnvironment _environment, string BodyHTML, string SubjectName)
        public static string PathPDF()
        {
            string _return = string.Empty;
            try
            {
                string pathToSave = string.Empty;
                FileName = IsReplace ?string.Format("{0}_{1}.pdf", Subject, DateTime.Now.Ticks.ToString()) : string.Format("{0}.pdf", Subject);
                #region Path
                if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
                {
                    _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }

                if (!string.IsNullOrEmpty(PathSave))
                {
                    pathToSave = Path.Combine(_environment.WebRootPath, PathSave);
                    DBpath = Path.Combine(PathSave, FileName);
                }
                else
                {
                    pathToSave = Path.Combine(_environment.WebRootPath, "temp");
                    DBpath = Path.Combine("temp", FileName);
                }

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                #endregion
                //string CustomCss = string.Empty;
                //string PathCss = Path.Combine(_environment.WebRootPath, "css", "bootstrap.min.css");
                //using (StreamReader reader = new StreamReader(PathCss))
                //{
                //    CustomCss = reader.ReadToEnd();
                //}

                
                string fileToSavePDF = Path.Combine(pathToSave, FileName);

                

                HtmlToPdf converter = new HtmlToPdf();
                converter.Options.PdfPageSize = PdfPageSize.A4;
                converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                //converter.Options.CustomCSS = CustomCss;
                converter.Options.CssMediaType = HtmlToPdfCssMediaType.Print;
                converter.Options.EmbedFonts = true;
                converter.Options.ExternalLinksEnabled = true;
                converter.Options.InternalLinksEnabled = true;
                converter.Options.JavaScriptEnabled = true;
                converter.Options.MinPageLoadTime = 3;
                //converter.Options.WebPageWidth = 1024;
                //converter.Options.WebPageHeight = 0;

                PdfDocument doc = converter.ConvertHtmlString(HTML);
                var log = converter.ConversionResult.ConsoleLog;

                // save pdf document
                //doc.Save(Response, false, "Sample.pdf");
                doc.Save(fileToSavePDF);

                // close pdf document
                doc.Close();

                _return = fileToSavePDF;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _return;
        }
    }
}
