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
        private SsUserRepo UserRepo;
        private IConfiguration config;
        private OptionTemplateRepo OptionRepo;
        public SsUserService(IConfiguration configuration)
        {
            config = configuration;
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



                string MvSpName = "vss_user";

                
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

                //if (dataDefineColumn != null)
                //{

                //    DefineColumn = dataDefineColumn.column_field;
                //    DefineColumnFormat = fn.FormatField(dataDefineColumn.column_field);

                //}
                //else
                //{
                //    DefineColumn = "no," + dataFieldList["Field"].ToString();
                //    DefineColumnFormat = fn.FormatField(dataFieldList["Field"].ToString());
                //    OptionRepo.InsertDefineColumn(user_id, subportfolio_id, option_url, line_no, DefineColumn);
                //}
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
                OutputList.DefineSize = DefineSize;
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
                Model.is_inactive = "N";
                Model.expired_date = DateTime.Now.AddYears(1);
                Model.subportfolio_id = Convert.ToInt32(EncryptionLibrary.DecryptText(Model.ss_subportfolio_id));
                Model.portfolio_id = Convert.ToInt32(EncryptionLibrary.DecryptText(Model.ss_portfolio_id));
                Model.group_id = Convert.ToInt32(EncryptionLibrary.DecryptText(Model.ss_group_id));
                Model.password = EncryptionLibrary.EncryptText(Model.password);
                Model.time_input = DateTime.Now;
                Model.time_edit = DateTime.Now;
                Model.user_input = EncryptionLibrary.DecryptText(Model.user_input);
                Model.user_edit = Model.user_input;
                UserRepo.Save(Model);
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
    }
}
