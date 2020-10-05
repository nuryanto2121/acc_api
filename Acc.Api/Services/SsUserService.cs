using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Interface;
using Acc.Api.Models;
using EncryptLibrary.AES256Encryption;
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
        private SsUserRepo UserRepo;
        private IConfiguration config;
        private OptionTemplateRepo OptionRepo;
        public SsUserService(IConfiguration configuration)
        {
            config = configuration;
            authRepo = new AuthRepo(Tools.ConnectionString(configuration));
            fn = new FunctionString(Tools.ConnectionString(configuration));
            UserRepo = new SsUserRepo(Tools.ConnectionString(configuration));
            OptionRepo = new OptionTemplateRepo(configuration);
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
        public Output GetMenuJson(string portfolio_id, string user_id)
        {
            Output _result = new Output();
            try
            {
                int? ss_portfolio_id = portfolio_id.ToLower() == "null" ? 0 : Convert.ToInt16(fn.DecryptString(portfolio_id));
                user_id = user_id.ToLower() == "null" ? null : fn.DecryptString(user_id);
                _result.Data = UserRepo.GetMenuJson(ss_portfolio_id, user_id);
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
                //Model.DataHeader.ss_group_id = Convert.ToInt32(fn.DecryptString(Model.group_id));

                UserRepo.DeleteDetailMenu(Convert.ToInt32(Model.portfolio_id), Model.user_id);

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
                UserRepo.Save(Model);

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
                Model.subportfolio_id = Convert.ToInt32(EncryptionLibrary.DecryptText(Model.ss_subportfolio_id));
                Model.portfolio_id = Convert.ToInt32(EncryptionLibrary.DecryptText(Model.ss_portfolio_id));
                Model.group_id = Convert.ToInt32(Model.ss_group_id);
                Model.time_edit = DateTime.Now;
                Model.user_edit = EncryptionLibrary.DecryptText(Model.user_edit);
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
                if(Model.CurrentPassword == Model.NewPassword)
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
    }
}
