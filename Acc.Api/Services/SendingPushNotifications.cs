using Acc.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Acc.Api.Services
{
    public static class SendingPushNotifications
    {
        private static Uri FireBasePushNotificationsURL = new Uri("https://fcm.googleapis.com/fcm/send");
        private static string ServerKey = "AAAAiH5N7BY:APA91bEdnKW3WPk7IqQa6gxvMLYoA6loRA8VtFHPfGHlXB2agrfrBGYep9UBRBXvRCyshBYmyJe0rVkhokELKTJmo6P5r91w9SS-XqbI6PW10wzCfcdoy28vWMnOEnZeyVO_1yRH58aO";

        public static async Task<bool> SendPushNotification(string[] deviceTokens, string title, string body, object data)
        {
            bool sent = false;
            try
            {

                if (deviceTokens.Count() > 0)
                {
                    //Object creation

                    var messageInformation = new Message()
                    {
                        notification = new Notification()
                        {
                            title = title,
                            text = body
                        },
                        data = data,
                        registration_ids = deviceTokens
                    };

                    //Object to JSON STRUCTURE => using Newtonsoft.Json;
                    string jsonMessage = JsonConvert.SerializeObject(messageInformation);

                    /*
                     ------ JSON STRUCTURE ------
                     {
                        notification: {
                                        title: "",
                                        text: ""
                                        },
                        data: {
                                action: "Play",
                                playerId: 5
                                },
                        registration_ids = ["id1", "id2"]
                     }
                     ------ JSON STRUCTURE ------
                     */

                    //Create request to Firebase API
                    var request = new HttpRequestMessage(HttpMethod.Post, FireBasePushNotificationsURL);

                    request.Headers.TryAddWithoutValidation("Authorization", "key=" + ServerKey);
                    request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                    HttpResponseMessage result;
                    using (var client = new HttpClient())
                    {
                        result = await client.SendAsync(request);
                        sent = sent && result.IsSuccessStatusCode;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           


            return sent;
        }

    }
}
