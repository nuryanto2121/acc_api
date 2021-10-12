using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Interface;
using Acc.Api.Models;
using EncryptLibrary.AES256Encryption;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class SsUserService : ICrudService<SsUser, int>
    {
        private FunctionString fn;
        private AuthRepo authRepo;
        private AuthServices authService;
        private SsUserRepo UserRepo;
        private IConfiguration config;
        private OptionTemplateRepo OptionRepo;
        public SsUserService(IConfiguration configuration, IEmailService EmailSender, IHostingEnvironment environment)
        {
            config = configuration;
            authRepo = new AuthRepo(Tools.ConnectionString(configuration));
            fn = new FunctionString(Tools.ConnectionString(configuration));
            UserRepo = new SsUserRepo(Tools.ConnectionString(configuration));
            OptionRepo = new OptionTemplateRepo(configuration);
            authService = new AuthServices(configuration, EmailSender, environment);
        }

        public Output Delete(int Key, int Timestamp)
        {
            Output _result = new Output();
            try
            {
                UserRepo.Delete(Key, Timestamp);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output GetDataBy(int Key, int Timestamp)
        {
            Output _result = new Output();
            try
            {
                var dtaUser = UserRepo.GetById(Key, Timestamp);
                dtaUser.password = null;
                _result.Data = dtaUser;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output GetDataList(string Parameter)
        {
            throw new NotImplementedException();
        }
        public Output GetMenuJson(string portfolio_id, string user_id, string group_access)
        {
            Output _result = new Output();
            try
            {
                int? ss_portfolio_id = portfolio_id.ToLower() == "null" ? 0 : Convert.ToInt16(fn.DecryptString(portfolio_id));
                user_id = user_id.ToLower() == "null" ? null : fn.DecryptString(user_id);
                _result.Data = UserRepo.GetMenuJson(ss_portfolio_id, user_id, group_access);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public Output Update(VmSsUser Model)
        {
            Output _result = new Output();
            try
            {
                Model.portfolio_id = fn.DecryptString(Model.portfolio_id);
                Model.user_input = fn.DecryptString(Model.user_input);
                Model.user_id = fn.DecryptString(Model.user_id);
                Model.group_id = fn.DecryptString(Model.group_id);
                //Model.DataHeader.ss_group_id = Convert.ToInt32(fn.DecryptString(Model.group_id));
                UserRepo.DeleteDashboardUser(Convert.ToInt32(Model.portfolio_id), Model.user_id);
                UserRepo.DeleteButtonUser(Convert.ToInt32(Model.portfolio_id), Model.user_id);
                UserRepo.DeleteDetailMenu(Convert.ToInt32(Model.portfolio_id), Model.user_id);
                UserRepo.UpdateDefaultDashboard(Model.user_id, Model.dashboard_url, Model.user_input);
                Model.DataDetail.ForEach(delegate (SsMenuUser dt)
                {
                    dt.ss_portfolio_id = Convert.ToInt32(fn.DecryptString(dt.portfolio_id));
                    dt.user_id = Model.user_id;
                    dt.user_input = Model.user_input;
                    dt.add_status = dt.add_status == null ? false : dt.add_status;
                    dt.edit_status = dt.edit_status == null ? false : dt.edit_status;
                    dt.delete_status = dt.delete_status == null ? false : dt.delete_status;
                    dt.view_status = dt.view_status == null ? false : dt.view_status;
                    dt.post_status = dt.post_status == null ? false : dt.post_status;
                    UserRepo.SaveDetail(dt);
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public DTResultListDyn<dynamic> GetList(ParamList JModel)
        {
            var OutputList = new DTResultListDyn<dynamic>();
            try
            {
                //if (JModel.Count == 0)
                //{
                //    throw new Exception("Data no valid.");
                //}
                //string option_url = JModel.option_url;
                //int line_no = JModel.line_no;
                string sWhere = string.Empty;
                //string user_id = fn.DecryptString(JModel.user_id);
                //string subportfolio_id = fn.DecryptString(JModel.subportfolio_id);
                int JmlahField = config.GetValue<int>("appSetting:JumlahFieldList");
                int iStart = JModel.current_page;
                int iPerPage = JModel.per_page;
                string sSortField = string.IsNullOrEmpty(JModel.sort_field) ? "ORDER BY time_edit desc" : "ORDER BY " + fn.FormatSort(JModel.sort_field);
                bool isViewFUnction = string.IsNullOrEmpty(JModel.param_view) ? false : true;



                string MvSpName = "vss_user_list";


                //var dataDefineColumn = OptionRepo.GetDefineColumn(user_id, subportfolio_id, option_url, line_no);

                var fieldsource = OptionRepo.getListFieldType(MvSpName, isViewFUnction);
                string AllColumn = fn.SetFieldList(fieldsource, fieldsource.Count)["Field"].ToString();
                JObject dataFieldList = fn.SetFieldList(fieldsource, JmlahField, definedColumn: AllColumn, List: true);

                string allCoulumnQUery = dataFieldList["FieldQuery"].ToString();
                string DefineSize = dataFieldList["DefineSize"].ToString();
                string fieldWhere = dataFieldList["fieldWhere"].ToString();

                #region Field
                string DefineColumn = string.Empty;
                string DefineColumnFormat = string.Empty;
                //cek defineColumn dlu

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
                OutputList.Data = UserRepo.QueryList(MvSpName, iStart, iPerPage, sSortField, sWhere); //allCoulumnQUery,DefineColumnFormat
                OutputList.Total = Convert.ToInt32(fn.SelectScalar(Enum.SQL.Function.Aggregate.Count, MvSpName, "*", sWhere));//Convert.ToInt32(this.CountList(MvSpName, sWhere));
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
                OutputList.DefineSize = "S,L,S,S,L";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return OutputList;
        }


        public Output Insert(SsUser Model)
        {
            Output _result = new Output();
            try
            {
                //check user_id exit
                string sParam = string.Format("user_id ilike '{0}'", Model.user_id);
                int userCNt = Convert.ToInt32(UserRepo.SelectScalar(Enum.SQL.Function.Aggregate.Count, "*", sParam));
                if (userCNt > 0)
                {
                    throw new Exception("Duplicate User ID.");
                }
                //hand_phone
                sParam = string.Format("hand_phone ilike '{0}' AND is_inactive = 'N'", Model.hand_phone);
                var ss_portfolio = UserRepo.SelectScalar(Enum.SQL.Function.Aggregate.Max, "portfolio_id", sParam);
                if (ss_portfolio != null)
                {
                    if (Convert.ToInt32(ss_portfolio) != Convert.ToInt32(fn.DecryptString(Model.ss_portfolio_id)))
                    {
                        throw new Exception("Duplicate Handphone in Another Portfolio.");
                    }
                    else
                    {
                        throw new Exception("Duplicate Handphone.");
                    }
                }


                JObject dataOut = new JObject();
                //Model.is_inactive = "N";
                Model.expired_date = DateTime.Now.AddYears(1);
                Model.subportfolio_id = Convert.ToInt32(fn.DecryptString(Model.ss_subportfolio_id));
                Model.portfolio_id = Convert.ToInt32(fn.DecryptString(Model.ss_portfolio_id));
                Model.group_id = Convert.ToInt32(fn.DecryptString(Model.ss_group_id));
                Model.password = EncryptionLibrary.EncryptText(Model.password);
                Model.time_input = DateTime.Now;
                Model.time_edit = DateTime.Now;
                Model.user_input = fn.DecryptString(Model.user_input);
                Model.user_edit = Model.user_input;
                //UserRepo.Save(Model);
                Model.ss_user_id = UserRepo.SaveNew(Model);

                if (Model.user_type == "I")
                {
                    UserRepo.InsertUserPorfolio(Model.user_id, Convert.ToInt32(Model.ss_group_id), Model.user_input);
                }

                dataOut.Add("row_id", Model.ss_user_id);
                _result.Data = dataOut;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output Update(SsUser Model)
        {
            Output _result = new Output();
            try
            {
                JObject dataOut = new JObject();
                if (string.IsNullOrEmpty(Model.password))
                {
                    SsUser dataUser = new SsUser();
                    dataUser = UserRepo.GetById(Model.ss_user_id, Model.lastupdatestamp);
                    Model.password = dataUser.password;//EncryptionLibrary.EncryptText(Model.password);
                }
                else
                {
                    Model.password = EncryptionLibrary.EncryptText(Model.password);
                }
                Model.subportfolio_id = Convert.ToInt32(fn.DecryptString(Model.ss_subportfolio_id));
                Model.portfolio_id = Convert.ToInt32(fn.DecryptString(Model.ss_portfolio_id));
                Model.group_id = Convert.ToInt32(Model.ss_group_id);
                Model.time_edit = DateTime.Now;
                Model.user_edit = fn.DecryptString(Model.user_edit);
                UserRepo.Update(Model);
                dataOut.Add("row_id", Model.ss_user_id);
                _result.Data = dataOut;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output ChangePassword(ChangePassword Model)
        {
            Output _result = new Output();
            Dictionary<string, object> DataUser = new Dictionary<string, object>();
            try
            {
                if (Model.ConfirmPassword != Model.NewPassword)
                {
                    throw new Exception("New Password and Confirm must be same.");
                }
                if (Model.CurrentPassword == Model.NewPassword)
                {
                    throw new Exception("Please Insert New Password.");
                }
                string User_id = EncryptionLibrary.DecryptText(Model.UserId);
                SsUser dataUser = authRepo.GetDataAuthByUserId(User_id);
                if (dataUser == null)
                {
                    throw new Exception("Account Not Valid.");
                }

                string OldPassword = EncryptionLibrary.EncryptText(Model.CurrentPassword);
                if (OldPassword != dataUser.password)
                {
                    throw new Exception("Password Not Valid.");
                }

                Model.NewPassword = EncryptionLibrary.EncryptText(Model.NewPassword);
                authRepo.UpdatePass(dataUser.ss_user_id, Model.NewPassword);
                _result.Message = "Change Password Successfuly.";
                //DataUser.Add("user_id", EncryptionLibrary.EncryptText(dataUser.user_id));
                //_result.Data = DataUser;
            }
            catch (Exception ex)
            {
                throw ex;
                //_result = Tools.Error(ex);
            }
            return _result;
        }
        public Output ChangePortfolio(ChangePortfolio Model)
        {
            Output _result = new Output();
            Dictionary<string, object> DataUser = new Dictionary<string, object>();
            try
            {

                Model.UserId = fn.DecryptString(Model.UserId);

                //update user porfolio
                //authRepo.UpdatePortfolio(Model.SsPortfolioId, Model.UserId);

                //get Data Login
                _result = this.DataLogin(Model);


                //logout user
                HttpContextAccessor Context = new HttpContextAccessor();
                var Headers = Context.HttpContext.Request.Headers;
                string Token = string.Empty;
                foreach (var key in Headers.Keys)
                {
                    if (key.ToLower() == "token")
                    {
                        Token = Headers[key].ToString();
                    }
                }

                var UserSession = new UserSession();
                UserSession.user_id = Model.UserId;
                UserSession.token = Token;
                UserSession.ip_address = Tools.GetIpAddress();
                authRepo.DeleteUserSession(UserSession);

                authRepo.UpdateUserLog(Model.UserId, Tools.GetIpAddress(), Token);

            }
            catch (Exception ex)
            {
                throw ex;
                //_result = Tools.Error(ex);
            }
            return _result;
        }

        private Output DataLogin(ChangePortfolio Model)
        {
            Output _result = new Output();
            string _Session_Id = string.Empty;
            AuthLogin Log = new AuthLogin();
            try
            {
                var UserSession = new UserSession();
                var SsUserLog = new SsUserLog();
                Dictionary<string, object> DataUser = new Dictionary<string, object>();
                Dictionary<string, object> ObjOutput = new Dictionary<string, object>();

                var dataAuth = authRepo.GetDataUser(Model.UserId);
                if (dataAuth.Rows.Count > 0)
                {
                    dataAuth.Rows[0]["portfolio_id"] = Model.SsPortfolioId;
                    //dataAuth.pwd = "";
                    //dataAuth.user_id = Tools.EncryptString(dataAuth.user_id
                    var MenuList = authService.menuList(dataAuth);
                    var FavMenu = authService.favoriteMenu(dataAuth);
                    if (MenuList.Rows.Count == 0)
                    {
                        throw new Exception("Please complete the menu assignment to user");
                    }
                    var expireDate = DateTime.Now.AddMinutes(config.GetValue<int>("appSetting:TokenExpire"));
                    _Session_Id = Token.GenerateToken(dataAuth, expireDate, Log, Tools.GetIpAddress());

                    // insert user session for authentication

                    UserSession.user_id = Model.UserId;
                    UserSession.expire_on = expireDate;
                    UserSession.token = _Session_Id;
                    UserSession.last_login = DateTime.Now;
                    UserSession.ip_address = Tools.GetIpAddress();
                    UserSession.user_input = Model.UserId;
                    UserSession.user_edit = Model.UserId;
                    UserSession.time_input = DateTime.Now;
                    UserSession.time_edit = DateTime.Now;
                    authRepo.SaveUserSession(UserSession);

                    // Insert User Log
                    authRepo.UpdateCaptchaLog(Tools.GetIpAddress());

                    SsUserLog.user_id = Model.UserId;
                    SsUserLog.ip_address = Tools.GetIpAddress();
                    SsUserLog.login_date = DateTime.Now;
                    SsUserLog.token = _Session_Id;
                    SsUserLog.is_fraud = false;
                    SsUserLog.user_input = Model.UserId;
                    SsUserLog.user_edit = Model.UserId;
                    SsUserLog.time_input = DateTime.Now;
                    SsUserLog.time_edit = DateTime.Now;
                    authRepo.SaveUserLog(SsUserLog);


                    
                    dataAuth = fn.DataClearEncrypt(dataAuth);

                    DataUser.Add("user_id", dataAuth.Rows[0]["user_id"].ToString());
                    DataUser.Add("user_name", dataAuth.Rows[0]["user_name"].ToString());
                    DataUser.Add("path_file", dataAuth.Rows[0]["path_file"].ToString());
                    DataUser.Add("group_id", dataAuth.Rows[0]["ss_group_id"].ToString());
                    DataUser.Add("dashboard_url", dataAuth.Rows[0]["dashboard_url"].ToString());
                    DataUser.Add("subportfolio_id", dataAuth.Rows[0]["subportfolio_id"].ToString());
                    DataUser.Add("subportfolio_short_name", dataAuth.Rows[0]["subportfolio_short_name"].ToString());
                    DataUser.Add("subportfolio_name", dataAuth.Rows[0]["subportfolio_name"].ToString());
                    DataUser.Add("portfolio_id", dataAuth.Rows[0]["portfolio_id"].ToString());
                    //DataUser.Add("portfolio_short_name", dataAuth.Rows[0]["portfolio_short_name"].ToString());
                    //DataUser.Add("portfolio_name", dataAuth.Rows[0]["portfolio_name"].ToString());
                    //DataUser.Add("portfolio_id", Tools.EncryptString(Model.SsPortfolioId.ToString()));
                    DataUser.Add("portfolio_short_name", Model.PortfolioName);
                    DataUser.Add("portfolio_name", Model.PortfolioName);

                    ObjOutput.Add("data_user", DataUser);
                    ObjOutput.Add("token", _Session_Id);
                    ObjOutput.Add("idle", config.GetValue<int>("appSetting:IdleWeb"));
                    ObjOutput.Add("menu", MenuList);
                    ObjOutput.Add("shorcut_menu", FavMenu);

                    _result.Data = ObjOutput;// dataAuth;
                }
                else
                {
                    throw new Exception("Invalid User.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
    }
}
