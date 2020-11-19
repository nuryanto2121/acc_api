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
        private UserFCMRepo userFcmRepo;
        public ChatSenderService(IConfiguration configuration)
        {
            fn = new FunctionString(Tools.ConnectionString(configuration));
            chatRepo = new ChatSenderRepo(Tools.ConnectionString(configuration));
            userFcmRepo = new UserFCMRepo(Tools.ConnectionString(configuration));
        }

        public Output GetAllChat(ChatSender Model)
        {
            Output _result = new Output();
            Dictionary<string, object> ObjOutput = new Dictionary<string, object>();
            try
            {
                Model.user_id_to = fn.DecryptString(Model.user_id_to);
                Model.portfolio_id = fn.DecryptString(Model.portfolio_id);
                Model.subportfolio_id = fn.DecryptString(Model.subportfolio_id);
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
                        chatRepo.UpdateStatusChatRead(dtChat.ss_chat_d_id, dtChat.user_id_from, Model.user_id_to);
                        dtChat.user_id_from = EncryptionLibrary.EncryptText(dtChat.user_id_from);
                    });
                    List<ChatListUser> Users = chatRepo.GetDataUserList(ChatId, Convert.ToInt32(Model.portfolio_id));// dataHedaer.user_id_to.Split(",").ToList();
                    string user_ids = string.Empty;
                    bool is_admin = false;
                    string user_names = string.Empty;
                    Users.ForEach(delegate (ChatListUser dt)
                    {
                        if (dt.is_admin)
                        {
                            if (Model.user_id_to == dt.user_id)
                            {
                                is_admin = true;
                            }
                        }
                        user_ids += dt.user_id + ","; //EncryptionLibrary.EncryptText(dt.user_id) + ",";
                        user_names += dt.user_name + ",";
                    });
                    //foreach (string dtUser in Users)
                    //{

                    //    user_ids += EncryptionLibrary.EncryptText(dtUser) + ",";
                    //}
                    user_ids = !string.IsNullOrEmpty(user_ids) ? user_ids.Remove(user_ids.LastIndexOf(",")) : user_ids;
                    user_names = !string.IsNullOrEmpty(user_names) ? user_names.Remove(user_names.LastIndexOf(",")) : user_names;
                    //ObjOutput.Add("user_from", EncryptionLibrary.EncryptText(dataHedaer.user_id_from));
                    ObjOutput.Add("subject", dataHedaer.subject);
                    ObjOutput.Add("user_names", user_names);
                    ObjOutput.Add("user_ids", user_ids);
                    ObjOutput.Add("row_id", HeaderId);
                    ObjOutput.Add("chat", dataChat);
                    ObjOutput.Add("your_id", Model.user_id_to);
                    ObjOutput.Add("is_admin", is_admin);
                    _result.Data = ObjOutput;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
        public async Task<Output> GetChatAsync(int id, string user_id)
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
                        dt.user_id_from = dt.user_id_from;//EncryptionLibrary.EncryptText(dt.user_id_from);
                    });

                }
                var dataHedaer = chatRepo.GetDataHeader(id);
                //ObjOutput.Add("user_from", EncryptionLibrary.EncryptText(dataHedaer.user_id_from));
                //List<string> Users = dataHedaer.user_id_to.Split(",").ToList();
                List<ChatListUser> Users = chatRepo.GetDataUserList(id, Tools.PortfolioId);// dataHedaer.user_id_to.Split(",").ToList();
                string user_ids = string.Empty;
                bool is_admin = false;
                Users.ForEach(delegate (ChatListUser dt)
                {
                    if (dt.is_admin)
                    {
                        if (user_id == dt.user_id)
                        {
                            is_admin = true;
                        }
                    }
                    user_ids += dt.user_id + ","; //EncryptionLibrary.EncryptText(dt.user_id) + ",";
                });
                //string user_ids = string.Empty;
                //foreach (string dtUser in Users)
                //{
                //    user_ids += EncryptionLibrary.EncryptText(dtUser) + ",";
                //}
                user_ids = !string.IsNullOrEmpty(user_ids) ? user_ids.Remove(user_ids.LastIndexOf(",")) : user_ids;
                ObjOutput.Add("subject", dataHedaer.subject);
                ObjOutput.Add("user_ids", user_ids);
                ObjOutput.Add("row_id", id);
                ObjOutput.Add("chat", dtList);
                ObjOutput.Add("is_admin", is_admin);
                ObjOutput.Add("your_id", user_id);
                _result.Data = ObjOutput;

                List<string> FCMToken = new List<string>();
                string[] TOkens = new string[1];
                TOkens[0] = "dbjfJKXWQw4dFm_cOByI-R:APA91bFlNjxf1N6uvnAwehIDEz276qOPvgU8yWB3CIGwd2LiqqZI17jr_TsHz5NiSlvxfhQNWa5Wx-F8TwNMSTQxc662Za_HhVqe2nGtI3hXAqxP7ODzGHMrw7b5HkgPc10FwfPWRF6N";

                //var dd = await SendingPushNotifications.SendPushNotification(TOkens, "Tes message", "ini adalah body", _result.Data);
                //var dtd = await SendingPushFCM.SendPushNotification2(TOkens, "Tes message FCM", "ini adalah body FCM", _result.Data);
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

        public Output DeleteHeader(int ID)
        {
            Output _result = new Output();
            try
            {
                _result.Data = chatRepo.DeleteChat(ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output RemoveUser(int ID, string UserRemove)
        {
            Output _result = new Output();
            try
            {
                List<ChatListUser> Users = chatRepo.GetDataUserList(ID, Tools.PortfolioId);
                var dd = Users.Where(w => w.user_id == Tools.UserId).FirstOrDefault();
                if (dd == null)
                {
                    throw new Exception("You don't have access for remove user.");
                }

                if (!dd.is_admin)
                {
                    throw new Exception("You don't have access for remove user.");
                }
                //
                UserRemove = fn.DecryptString(UserRemove);
                _result.Data = chatRepo.RemoveUser(ID, UserRemove);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public Output AddUser(int ID, string AddUser)
        {
            Output _result = new Output();
            try
            {
                List<ChatListUser> Users = chatRepo.GetDataUserList(ID, Tools.PortfolioId);
                var dd = Users.Where(w => w.user_id == Tools.UserId).FirstOrDefault();
                if (dd == null)
                {
                    throw new Exception("You don't have access for remove user.");
                }

                if (!dd.is_admin)
                {
                    throw new Exception("You don't have access for remove user.");
                }
                chatRepo.RemoveUserAll(ID);
                //
                List<string> AddUsers = AddUser.Split(",").ToList();
                foreach (string UserID in AddUsers)
                {
                    string user_id = fn.DecryptString(UserID);
                    _result.Data = chatRepo.AddUser(ID, user_id);
                }


                //_result.Data = chatRepo.RemoveUser(ID, AddUser);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public async Task<Output> SendChatAsync(ChatDetail Model)
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
                //List<string> Users = dataHedaer.user_id_to.Split(",").ToList();
                List<ChatListUser> Users = chatRepo.GetDataUserList(Model.ss_chat_h_id, Tools.PortfolioId);// dataHedaer.user_id_to.Split(",").ToList();

                if (!Users.Any(a => a.user_id == Model.user_id_from))
                {
                    throw new Exception("You cant's send message because you're no longer participant.");
                }

                #region save detail chat
                var ddd = Users.Where(w => w.user_id != Model.user_id_from).Select(s => s.user_id).ToList();
                Model.user_id_to = string.Join(",", ddd);
                Model.ss_chat_attachment_id = null;
                var _ret = chatRepo.SendChat(Model);
                #endregion                


                string user_ids = string.Empty;
                Users.ForEach(delegate (ChatListUser dt)
                {

                    if (Model.user_id_from != dt.user_id)
                    {
                        
                        ChatDetail dtDetail = new ChatDetail();
                        dtDetail.ss_chat_d_id = _ret.row_id;
                        dtDetail.user_id_from = Model.user_id_from;
                        dtDetail.user_id_to = dt.user_id;
                        dtDetail.user_input = Model.user_input;
                        chatRepo.SendChatRead(dtDetail);
                    }

                });

                #region send to fcm
                var DataUserFCM = userFcmRepo.GetList(Model.ss_chat_h_id, Model.user_id_from);

                if (DataUserFCM.Count > 0)
                {
                    //string[] Tokens = new string[DataUserFCM.Count];
                    var chatNotif = userFcmRepo.GetSumChatNotif(Tools.PortfolioId, Tools.UserId);
                    var Tokens = DataUserFCM.Select(s => s.fcm_token).ToArray();
                    //var dd = await SendingPushNotifications.SendPushNotification(Tokens, dataHedaer.subject, Model.chat_text, chatNotif);
                    var dd = await SendingPushFCM.SendPushNotification2(Tokens, dataHedaer.subject, Model.chat_text, chatNotif);
                }
                #endregion




                _result.Message = "Success.";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }

        public async Task<Output> SaveAttachment(ChatAttachment Model)
        {
            Output _result = new Output();
            try
            {
                var AttachID = chatRepo.SendAttachment(Model);

                ChatDetail SendChat = new ChatDetail();

                // check header
                var dataHedaer = chatRepo.GetDataHeader(Model.ss_chat_h_id);

                //string User = dataHedaer.user_id_to + "," + dataHedaer.user_id_from;
                //List<string> Users = dataHedaer.user_id_to.Split(",").ToList();
                List<ChatListUser> Users = chatRepo.GetDataUserList(Model.ss_chat_h_id, Tools.PortfolioId);// dataHedaer.user_id_to.Split(",").ToList();
                var UsersTo = Users.Select(s => s.user_id).Aggregate((i, j) => i + "," + j);

                SendChat.ss_chat_h_id = Model.ss_chat_h_id;
                SendChat.chat_text = Model.path_file;
                SendChat.chat_date = Model.chat_date;
                SendChat.user_id_from = Tools.UserId;
                SendChat.user_id_to = UsersTo;
                SendChat.is_file = true;
                SendChat.ss_chat_attachment_id = AttachID.row_id;
                SendChat.user_input = Tools.UserId;
                var _ret = chatRepo.SendChat(SendChat);


                if (!Users.Any(a => a.user_id == Tools.UserId))
                {
                    throw new Exception("You cant's send message because you're no longer participant.");
                }

                Users.ForEach(delegate (ChatListUser dt)
                {

                    if (Tools.UserId != dt.user_id)
                    {
                        ChatDetail dtDetail = new ChatDetail();
                        dtDetail.ss_chat_d_id = _ret.row_id;
                        dtDetail.user_id_from = Tools.UserId;
                        dtDetail.user_id_to = dt.user_id;
                        dtDetail.user_input = Tools.UserId;
                        chatRepo.SendChatRead(dtDetail);
                    }

                });

                #region send to fcm
                var DataUserFCM = userFcmRepo.GetList(Model.ss_chat_h_id, Tools.UserId);

                if (DataUserFCM.Count > 0)
                {
                    //string[] Tokens = new string[DataUserFCM.Count];
                    var chatNotif = userFcmRepo.GetSumChatNotif(Tools.PortfolioId, Tools.UserId);
                    var Tokens = DataUserFCM.Select(s => s.fcm_token).ToArray();
                    //var dd = await SendingPushNotifications.SendPushNotification(Tokens, dataHedaer.subject, Model.file_name, chatNotif);
                    var dd = await SendingPushFCM.SendPushNotification2(Tokens, dataHedaer.subject, Model.file_name, chatNotif);
                }
                #endregion


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
    }
}
