using EncryptLibrary.AES256Encryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Npgsql;
using Acc.Api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Acc.Api.Helper
{
    public class Tools
    {
        //private readonly IHostingEnvironment env;
        //public Tools(IHostingEnvironment env)
        //{
        //    if (env == null)
        //        throw new ArgumentNullException(nameof(env));

        //    this.env = env;
        //}
        /// <summary>
        /// ini utk connection postgress
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public static IDbConnection DBConnection(string ConnectionString)
        {
            return new NpgsqlConnection(ConnectionString);
        }
        /// <summary>
        /// Ini utk Connecti SQL
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public static IDbConnection SqlConnection(string ConnectionString)
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Connection Mysql
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public static IDbConnection MySqlConnection(string ConnectionString)
        {
            return new MySqlConnection(ConnectionString);
        }
        public static string UserId
        {
            get
            {
                HttpContextAccessor dd = new HttpContextAccessor();
                var Headers = dd.HttpContext.Request.Headers;
                string Token = string.Empty;
                foreach (var key in Headers.Keys)
                {
                    if (key.ToLower() == "token")
                    {
                        Token = Headers[key].ToString();
                    }
                }
                var Key = EncryptionLibrary.DecryptText(Token);
                string[] Parts = Key.Split(new string[] { "~!@#$%^" }, StringSplitOptions.None);
                return Parts[0];
            }

        }

        public static int PortfolioId
        {
            get
            {
                HttpContextAccessor dd = new HttpContextAccessor();
                var Headers = dd.HttpContext.Request.Headers;
                string Token = string.Empty;
                foreach (var key in Headers.Keys)
                {
                    if (key.ToLower() == "token")
                    {
                        Token = Headers[key].ToString();
                    }
                }
                var Key = EncryptionLibrary.DecryptText(Token);
                string[] Parts = Key.Split(new string[] { "~!@#$%^" }, StringSplitOptions.None);
                return Convert.ToInt32(Parts[1]);
            }
        }
        public static string DecryptString(string data)
        {
            string _result = data;
            if (isBase64(data))
            {
                _result = EncryptionLibrary.DecryptText(data);
            }
            return _result;
        }
        public static string EncryptString(string data)
        {

            return EncryptionLibrary.EncryptText(data);

        }
        public static bool isBase64(string Base64)
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
        public static string ConnectionString(IConfiguration configuration)
        {
            return configuration.GetValue<string>("appSetting:ConnectionString");
        }
        public static string GetIpAddress()
        {
            HttpContextAccessor dd = new HttpContextAccessor();
            return dd.HttpContext.Connection.RemoteIpAddress.ToString();
        }
        public static string GetUserAgent()
        {
            HttpContextAccessor dd = new HttpContextAccessor();
            var Headers = dd.HttpContext.Request.Headers;
            return Headers["User-Agent"].ToString();
        }
        public static Output Error(Exception ex, int statusCode = 500)
        {
            Output op = new Output();
            op.Message = ex.Message;
            op.Status = statusCode;
            op.Error = true;
            return op;
        }
        public static Output Error(string Message, int statusCode = 500)
        {
            Output op = new Output();
            op.Message = Message;
            op.Status = statusCode;
            op.Error = true;
            return op;
        }
        public static Output Error(Exception ex, HttpStatusCode statusCode)
        {
            //ObjectResult ops = new ObjectResult();
            Output op = new Output();
            op.Message = ex.Message;
            op.Status = Convert.ToInt32(statusCode);
            op.Error = true;

            return op;
        }
        public static ErrorStatusCode ErrStatusCode(Exception ex)
        {
            ErrorStatusCode _result = new ErrorStatusCode();
            _result.StatusCode = StatusCodes.Status500InternalServerError;
            _result.Message = ex.Message+"\n"+ex.StackTrace;
            if (ex.Message.IndexOf("^") != -1)
            {
                string []msg = ex.Message.Split("^");
                _result.Message = msg[0];
                _result.StatusCode = Convert.ToInt32(msg[1]);                

            }
            return _result;
        }
        public static ErrorStatusCode ErrStatusCode(string Msg)
        {
            ErrorStatusCode _result = new ErrorStatusCode();
            _result.StatusCode = StatusCodes.Status500InternalServerError;
            _result.Message = Msg;
            if (Msg.IndexOf("^") != -1)
            {
                string[] msg = Msg.Split("^");
                _result.Message = msg[0];
                _result.StatusCode = Convert.ToInt32(msg[1]);

            }
            return _result;
        }
        public static Output Error(string ex)
        {

            Output op = new Output();
            op.Message = ex;
            op.Status = 500;
            op.Error = true;
            return op;
        }
        public static string GetString(string strSource, string strStart, string strEnd)
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
        public static void TextError(IHostingEnvironment env,Exception error)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(env.WebRootPath))
                {
                    env.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }
                string pathToSave = Path.Combine(env.WebRootPath, "error");
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
        public static void writeFile(string[] data, string pathFile)
        {         
            try
            {
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(pathFile))
                {
                    foreach (string line in data)
                    {

                        file.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static Exception SetError(string msg, HttpStatusCode code)
        {
            var ex = new Exception(string.Format("{0} - {1}", msg, code));
            ex.Data.Add(code, msg);
            return ex;
        }
    }
    public static class Token
    {
        public static string GenerateToken(SsUser dataUser, DateTime ExpireToken, AuthLogin Log, string IpAddress)
        //public static string GenerateToken(DataTable dtTableLogin, DateTime ExpireToken, AuthLog Log)
        {
            try
            {
                string randomnumber = string.Join("~!@#$%", new string[] {
                    dataUser.user_id,
                    EncryptionLibrary.KeyGenerator.GetUniqueKey(),
                    dataUser.password,
                    IpAddress,
                    Convert.ToString(DateTime.Now.Ticks),
                    Convert.ToString(ExpireToken.Ticks),
                    dataUser.email
                    //dataUser.Rows[0]["user_group"].ToString()

                });

                return EncryptionLibrary.EncryptText(randomnumber);
                //return SymmCrypto.Encrypt(randomnumber, PAMSCrypto.CryptWord);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static string GenerateToken(DataTable dtTableLogin, DateTime ExpireToken, AuthLogin Log, string IpAddress)
        //public static string GenerateToken(DataTable dtTableLogin, DateTime ExpireToken, AuthLog Log)
        {
            try
            {
                string randomnumber = string.Join("~!@#$%^", new string[] {
                    dtTableLogin.Rows[0]["user_id"].ToString(),
                    dtTableLogin.Rows[0]["portfolio_id"].ToString(),
                    dtTableLogin.Rows[0]["password"].ToString(),
                    IpAddress,
                    Convert.ToString(DateTime.Now.Ticks),
                    Convert.ToString(ExpireToken.Ticks),
                    dtTableLogin.Rows[0]["user_group_short_descs"].ToString()

                });

                return EncryptionLibrary.EncryptText(randomnumber);
                //return SymmCrypto.Encrypt(randomnumber, PAMSCrypto.CryptWord);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
