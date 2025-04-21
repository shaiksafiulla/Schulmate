namespace SchoolCoreAPI.Hub
{
    public interface IMessageHubClient
    {
        Task SendOffersToUser(List<string> message);
        //Task SendMessage(string user, string message);
        //Task SendNotification(string message);
    }
}
