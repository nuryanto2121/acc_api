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
        public static async Task<bool> SendPushNotification2(string[] deviceTokens, string title, string body, object data)
        {
            var pathFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Key-FCM.json");
            var defaultApp = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(pathFile),
            });
            Console.WriteLine(defaultApp.Name); // "[DEFAULT]"
            bool sent = false;
            var message = new Message()
            {
                Data = new Dictionary<string, string>()
                {
                    ["FirstName"] = "John",
                    ["LastName"] = "Doe"
                },
                //Data = data,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },

                //Token = "d3aLewjvTNw:APA91bE94LuGCqCSInwVaPuL1RoqWokeSLtwauyK-r0EmkPNeZmGavSG6ZgYQ4GRjp0NgOI1p-OAKORiNPHZe2IQWz5v1c3mwRE5s5WTv6_Pbhh58rY0yGEMQdDNEtPPZ_kJmqN5CaIc",
                Topic = "news",
                
                //Token = "dbjfJKXWQw4dFm_cOByI-R:APA91bFlNjxf1N6uvnAwehIDEz276qOPvgU8yWB3CIGwd2LiqqZI17jr_TsHz5NiSlvxfhQNWa5Wx-F8TwNMSTQxc662Za_HhVqe2nGtI3hXAqxP7ODzGHMrw7b5HkgPc10FwfPWRF6N",
                Token = "AAAAiH5N7BY:APA91bEYAyJ9tMvG56c-XTW90V1sxcBhNxeNCs_z_S9YvSnHzZVPIz9BrhT8EOMRp2wvgec6acLpZlE915i0H0UUQe8vX8MsJxLoAOihzLB4ro5-EJhOsAz0jqXMk9cG27wnWCsPD0x_"

            };
            var messaging = FirebaseMessaging.DefaultInstance;
            var result = await messaging.SendAsync(message);
            Console.WriteLine(result); //projects/myapp/messages/2492588335721724324

            return sent;
        }
    }
}
