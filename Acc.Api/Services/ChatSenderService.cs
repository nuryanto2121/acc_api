using Acc.Api.DataAccess;
using Acc.Api.Helper;
using Acc.Api.Models;
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
                    _result.Data = chatRepo.GetAllChat(ChatId);
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
            try
            {
                GetChat dt = new GetChat();
                user_id = fn.DecryptString(user_id);

                dt = chatRepo.GetChat(id, user_id);

                chatRepo.UpdateStatusChat(dt);

                _result.Data = dt;
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
