using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using TheAnimalBot.Dialogs;

namespace TheAnimalBot.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {

                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                userData.SetProperty("latest_message", activity.Text);
                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);


                await Conversation.SendAsync(activity, () => new AnimalDialog());
            } else if(activity.Type == ActivityTypes.ConversationUpdate)
            {
                if(activity.MembersAdded.Count > 0 && activity.MembersAdded[0].Name.Equals("AnimalBot"))
                {
                    ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    var retter = activity.CreateReply($"Welcome! My name is AnimalBot, but you can call me Tim. Don't be scared to message me!");
                    await connector.Conversations.ReplyToActivityAsync(retter);
                    await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(AnimalDialog.HelpString));
                }
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}