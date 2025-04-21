using SchoolCoreAPI.Models;


namespace SchoolCoreAPI.IServices
{
    public interface IPushSubscriptionService
    {
        Task<ServiceResult> Subscribe(pushsubscriptions model);
        Task<ServiceResult> UnSubscribe(int userId);
        //Task SendNotificationAsync(PushSubscription subscription, string payload);
        Task<ServiceResult> SendPushNotificationAsync(PushPayLoad payload, IEnumerable<int>? targetUserIds = null);
        //Task<int> SendPushNotificationAsync(string message, IEnumerable<int>? targetUserIds = null);

        

        void Dispose();
    }
}
