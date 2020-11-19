using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public static class SendingPushFCM
    {
        //var defaultApp = FirebaseApp.Create(new AppOptions(){Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.json")) });

        //Console.WriteLine(defaultApp.Name); // "[DEFAULT]"
        public static async Task<bool> SendPushNotification2(string[] deviceTokens, string title, string body, Models.ChatNotif data)
        {
            bool sent = false;
            try
            {
                var pathFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Key-FCM.json");
                AppOptions options = new AppOptions();
                options.Credential = GoogleCredential.FromFile(pathFile);
                var dd = FirebaseApp.DefaultInstance;
                if (dd == null)
                {
                    var defaultApp = FirebaseApp.Create(options);
                    Console.WriteLine(defaultApp.Name); // "[DEFAULT]"
                }



                //var defaultApp = FirebaseApp.Create(new AppOptions()
                //{
                //    Credential = GoogleCredential.FromFile(pathFile),
                //});
                

                var message = new MulticastMessage()
                {
                    Data = new Dictionary<string, string>()
                    {
                        ["v_chat"] = data.v_chat.ToString(),
                        ["v_notif"] = data.v_notif.ToString()
                    },
                    //Data = data,
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    },

                    //Token = "d3aLewjvTNw:APA91bE94LuGCqCSInwVaPuL1RoqWokeSLtwauyK-r0EmkPNeZmGavSG6ZgYQ4GRjp0NgOI1p-OAKORiNPHZe2IQWz5v1c3mwRE5s5WTv6_Pbhh58rY0yGEMQdDNEtPPZ_kJmqN5CaIc",
                    //Topic = "news",

                    Tokens = deviceTokens,//"/RLW7i95EwlV2CkyUNVcanlcBvv228ijPVsG1Nydf/GNhmnWMjGnzsMO8hZ+suQuJVYlGwoVSo5+EyjlcD5+WQ7Z7pszBNf2CiiSHnuWKnkWIMLN4XRqAHtabaLnwXxW43+qISFCz1XcYd3kmC+LmEZY6AEJDORA4uTvIvuBP2qqGn5QLD6QjEDzRyWFa0I29wY925Yf09c7KEgQklXlgw==",
                                          //Token = "AAAAiH5N7BY:APA91bEdnKW3WPk7IqQa6gxvMLYoA6loRA8VtFHPfGHlXB2agrfrBGYep9UBRBXvRCyshBYmyJe0rVkhokELKTJmo6P5r91w9SS-XqbI6PW10wzCfcdoy28vWMnOEnZeyVO_1yRH58aO"

                };
                var messaging = FirebaseMessaging.DefaultInstance;
                var result = await messaging.SendMulticastAsync(message);
                Console.WriteLine(result); //projects/myapp/messages/2492588335721724324
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return sent;
        }
    }
}
