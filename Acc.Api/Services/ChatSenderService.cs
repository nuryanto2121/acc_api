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
                string paramWhere = string.Format("doc_type = '{0}' AND doc_no = '{1}' AND ss_portfolio_id='{2}' AND ss_subportfolio_id='{3}'",
                                                    Model.doc_type, Model.doc_no, Model.portfolio_id, Model.subportfolio_id);
                paramWhere = fn.DecryptDataString(paramWhere);
                var HeaderId = fn.SelectScalar(Enum.SQL.Function.Aggregate.Max, "ss_chat_h", "ss_chat_h_id", paramWhere);
                if (HeaderId != null)
                {
                    int ChatId = Convert.ToInt32(HeaderId);
                    var dataHedaer = chatRepo.GetDataHeader(ChatId);
                    var dataChat = chatRepo.GetAllChat(ChatId);
                    dataChat.ForEach(delegate (GetChat dtChat)
                    {
                        dtChat.user_id_from = EncryptionLibrary.EncryptText(dtChat.user_id_from);
                    });
                    ObjOutput.Add("user_from", EncryptionLibrary.EncryptText(dataHedaer.user_id_from));
                    ObjOutput.Add("user_to", EncryptionLibrary.EncryptText(dataHedaer.user_id_to));
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
                        chatRepo.UpdateStatusChat(dt);
                        dt.user_id_from = EncryptionLibrary.EncryptText(dt.user_id_from);
                    });

                }
                var dataHedaer = chatRepo.GetDataHeader(id);
                ObjOutput.Add("user_from", EncryptionLibrary.EncryptText(dataHedaer.user_id_from));
                ObjOutput.Add("user_to", EncryptionLibrary.EncryptText(dataHedaer.user_id_to));
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
                chatRepo.SendChat(Model);
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
