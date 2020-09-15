using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Models;
using EncryptLibrary.AES256Encryption;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public class ChatSenderService
    {
        private FunctionString fn;
        private ChatSenderRepo chatRepo;
        public ChatSenderService(IConfiguration configuration)
        {
            fn = new FunctionString(Tools.ConnectionString(configuration));
            chatRepo = new ChatSenderRepo(Tools.ConnectionString(configuration));
        }

        public Output GetAllChat(ChatSender Model)
        {
            Output _result = new Output();
            Dictionary<string, object> ObjOutput = new Dictionary<string, object>();
            try
            {
                //check chat header
                //string paramWhere = string.Format("doc_type = '{0}' AND doc_no = '{1}' AND ss_portfolio_id='{2}' AND ss_subportfolio_id='{3}'",
                //                                    Model.doc_type, Model.doc_no, Model.portfolio_id, Model.subportfolio_id);
                string paramWhere = string.Format("doc_type = '{0}' AND row_id = {1} AND ss_portfolio_id='{2}' AND ss_subportfolio_id='{3}'",
                                                    Model.doc_type, Model.row_id, Model.portfolio_id, Model.subportfolio_id);
                paramWhere = fn.DecryptDataString(paramWhere);
                var HeaderId = fn.SelectScalar(Enum.SQL.Function.Aggregate.Max, "ss_chat_h", "ss_chat_h_id", paramWhere);
                if (HeaderId != null)
                {
                    int ChatId = Convert.ToInt32(HeaderId);
                    var dataHedaer = chatRepo.GetDataHeader(ChatId);
                    var dataChat = chatRepo.GetAllChat(ChatId, Model.current_page);
                    dataChat.ForEach(delegate (GetChat dtChat)
                    {
                        dtChat.user_id_from = EncryptionLibrary.EncryptText(dtChat.user_id_from);
                    });
                    List<string> Users = dataHedaer.user_id_to.Split(",").ToList();
                    string user_ids = string.Empty;
                    foreach (string dtUser in Users)
                    {
                        user_ids += EncryptionLibrary.EncryptText(dtUser) + ",";
                    }
                    user_ids = !string.IsNullOrEmpty(user_ids) ? user_ids.Remove(user_ids.LastIndexOf(",")) : user_ids;
                    //ObjOutput.Add("user_from", EncryptionLibrary.EncryptText(dataHedaer.user_id_from));
                    ObjOutput.Add("subject", dataHedaer.subject);
                    ObjOutput.Add("user_ids", user_ids);
                    ObjOutput.Add("row_id", HeaderId);
                    ObjOutput.Add("chat", dataChat);
                    _result.Data = ObjOutput;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public Output GetChat(int id, string user_id)
        {
            Output _result = new Output();
            Dictionary<string, object> ObjOutput = new Dictionary<string, object>();
            try
            {
                List<GetChat> dtList = new List<GetChat>();
                user_id = fn.DecryptString(user_id);

                dtList = chatRepo.GetChat(id, user_id);
                if (dtList.Count > 0)
                {
                    dtList.ForEach(delegate (GetChat dt)
                    {
                        chatRepo.UpdateStatusChatRead(dt.ss_chat_d_id, dt.user_id_from, user_id);
                        dt.user_id_from = EncryptionLibrary.EncryptText(dt.user_id_from);
                    });

                }
                var dataHedaer = chatRepo.GetDataHeader(id);
                //ObjOutput.Add("user_from", EncryptionLibrary.EncryptText(dataHedaer.user_id_from));
                List<string> Users = dataHedaer.user_id_to.Split(",").ToList();
                string user_ids = string.Empty;
                foreach (string dtUser in Users)
                {
                    user_ids += EncryptionLibrary.EncryptText(dtUser) + ",";
                }
                user_ids = !string.IsNullOrEmpty(user_ids) ? user_ids.Remove(user_ids.LastIndexOf(",")): user_ids;
                ObjOutput.Add("subject", dataHedaer.subject);
                ObjOutput.Add("user_ids", user_ids);
                ObjOutput.Add("row_id", id);
                ObjOutput.Add("chat", dtList);
                _result.Data = ObjOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public Output SaveHeader(ChatSender Model)
        {
            Output _result = new Output();
            try
            {
                Model.portfolio_id = fn.DecryptString(Model.portfolio_id);
                Model.subportfolio_id = fn.DecryptString(Model.subportfolio_id);
                Model.user_input = fn.DecryptString(Model.user_input);

                Model.user_id_from = fn.DecryptString(Model.user_id_from);
                Model.user_id_to = fn.DecryptString(Model.user_id_to) + "," + Model.user_id_from;
                _result.Data = chatRepo.SaveHeader(Model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output SendChat(ChatDetail Model)
        {
            Output _result = new Output();
            try
            {
                string paramWhere = string.Format("ss_chat_h_id = {0}", Model.ss_chat_h_id);
                //string user_id_to = fn.SelectScalar(Enum.SQL.Function.Aggregate.Max, "ss_chat_h", "user_id_to", paramWhere).ToString();
                Model.user_id_to = fn.DecryptString(Model.user_id_to);
                Model.user_id_from = fn.DecryptString(Model.user_id_from);
                Model.user_input = fn.DecryptString(Model.user_input);

                // check header
                var dataHedaer = chatRepo.GetDataHeader(Model.ss_chat_h_id);
                //string User = dataHedaer.user_id_to + "," + dataHedaer.user_id_from;
                List<string> Users = dataHedaer.user_id_to.Split(",").ToList();

                if (!Users.Any(dt => dt == Model.user_id_from))
                {
                    Users.Add(Model.user_id_from);
                    dataHedaer.user_id_to = string.Join(",", Users);//string.Format(",{0}", Model.user_id_from);
                    chatRepo.UpdateUserHeader(Model.ss_chat_h_id, dataHedaer.user_id_to);


                }
                List<string> UserTO = Users;
                UserTO.Remove(Model.user_id_from);

                Model.user_id_to = string.Join(",", UserTO);
                var _ret = chatRepo.SendChat(Model);

                foreach (string dtUserTo in UserTO)
                {

                    ChatDetail dtDetail = new ChatDetail();
                    dtDetail.ss_chat_d_id = _ret.row_id;
                    dtDetail.user_id_from = Model.user_id_from;
                    dtDetail.user_id_to = dtUserTo;
                    dtDetail.user_input = Model.user_input;
                    chatRepo.SendChatRead(dtDetail);

                }
                _result.Message = "Success.";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
    }
}
