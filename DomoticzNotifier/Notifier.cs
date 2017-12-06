using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using PushbulletSharp;
using PushbulletSharp.Models.Requests;

namespace DomoticzNotifier
{
    public static class Notifier
    {
        [FunctionName("Notifier")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                NotifyRequest notifyRequest = await req.Content.ReadAsAsync<NotifyRequest>();

                string[] pushbulletAccessTokens = Environment.GetEnvironmentVariable("PushbulletAccessTokens").Split(';');

                foreach (var pushbulletAccessToken in pushbulletAccessTokens)
                {
                    var pushbulletClient = new PushbulletClient(pushbulletAccessToken);
                    var result = pushbulletClient.PushNote(new PushNoteRequest()
                    {
                        Body = notifyRequest.message,
                        Title = notifyRequest.subject
                    });
                }

                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch(Exception e)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}
