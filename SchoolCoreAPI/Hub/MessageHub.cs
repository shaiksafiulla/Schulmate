using Microsoft.AspNetCore.SignalR;

namespace SchoolCoreAPI.Hub
{
    public class MessageHub : Hub<IMessageHubClient>
    {
        public async Task SendOffersToUser(List<string> message)
        {
            await Clients.All.SendOffersToUser(message);
        }

        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendMessage("ReceiveMessage", user, message);
        //}

        //public async Task SendNotification(string message)
        //{
        //    await Clients.All.SendAsync("ReceiveNotification", message);
        //}
    }
}
