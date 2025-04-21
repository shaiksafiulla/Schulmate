using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using WebPush;

using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class PushSubscriptionService : IPushSubscriptionService
    {
        private readonly APIContext _context;
        private readonly IVapidKeyService _vapidKeyService;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        public PushSubscriptionService(APIContext context, IVapidKeyService vapidKeyService, ILogger<RequestLoggingMiddleware> logger)
        {
            _context = context;
            _vapidKeyService = vapidKeyService;
            _logger = logger;
        }

        public async Task<ServiceResult> Subscribe(pushsubscriptions model)
        {
            ServiceResult result = null;
            try
            {
                var existingSubscription = await _context.pushsubscriptions
                    .FirstOrDefaultAsync(s => s.CreatedByUserId == model.CreatedByUserId);

                if (existingSubscription != null)
                {
                    bool isUpdated = false;

                    if (existingSubscription.IsActive != ((int)ActiveState.Active).ToString())
                    {
                        existingSubscription.IsActive = ((int)ActiveState.Active).ToString();
                        isUpdated = true;
                    }
                    if (existingSubscription.EndPoint != model.EndPoint)
                    {
                        existingSubscription.EndPoint = model.EndPoint;
                        isUpdated = true;
                    }
                    if (existingSubscription.P256dh != model.P256dh)
                    {
                        existingSubscription.P256dh = model.P256dh;
                        isUpdated = true;
                    }
                    if (existingSubscription.Auth != model.Auth)
                    {
                        existingSubscription.Auth = model.Auth;
                        isUpdated = true;
                    }

                    if (isUpdated)
                    {
                        existingSubscription.ModifiedDate = DateTime.Now;
                        _context.pushsubscriptions.Update(existingSubscription);
                        int updateIndex = await _context.SaveChangesAsync();

                        if (updateIndex > 0)
                        {
                            result = new ServiceResult
                            {
                                StatusId = (int)ServiceResultStatus.Updated,
                                RecordId = existingSubscription.Id
                            };
                        }
                    }
                    else
                    {
                        result = new ServiceResult
                        {
                            StatusId = (int)ServiceResultStatus.Updated,
                            RecordId = existingSubscription.Id
                        };
                    }
                }
                else
                {
                    var newSubscription = new pushsubscriptions
                    {
                        EndPoint = model.EndPoint,
                        P256dh = model.P256dh,
                        Auth = model.Auth,
                        IsActive = ((int)ActiveState.Active).ToString(),
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = model.CreatedByUserId,
                        ModifiedDate = DateTime.Now
                    };

                    _context.pushsubscriptions.Add(newSubscription);
                    int addedIndex = await _context.SaveChangesAsync();

                    if (addedIndex > 0)
                    {
                        result = new ServiceResult
                        {
                            StatusId = (int)ServiceResultStatus.Added,
                            RecordId = newSubscription.Id
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
            }

            return result;
        }

        public async Task<ServiceResult> UnSubscribe(int userId)
        {
            ServiceResult result = null;
            try
            {
                var existingSubscription = await _context.pushsubscriptions
                    .FirstOrDefaultAsync(s => s.CreatedByUserId == userId);

                if (existingSubscription != null)
                {
                    existingSubscription.IsActive = ((int)ActiveState.InActive).ToString();
                    existingSubscription.ModifiedDate = DateTime.Now;
                    _context.pushsubscriptions.Update(existingSubscription);
                    int updateIndex = await _context.SaveChangesAsync();

                    if (updateIndex > 0)
                    {
                        result = new ServiceResult
                        {
                            StatusId = (int)ServiceResultStatus.Updated,
                            RecordId = existingSubscription.Id
                        };
                    }
                }
                else
                {
                    result = new ServiceResult
                    {
                        StatusId = (int)ServiceResultStatus.Updated,
                        RecordId = 0
                    };
                }
            }
            catch (Exception ex)
            {
                // Log the exception
            }

            return result;
        }

        //public async Task<ServiceResult> SendPushNotificationAsync(PushPayLoad payload, IEnumerable<int>? targetUserIds = null)
        //{
        //    var (publicKey, privateKey) = _vapidKeyService.GetKeysFromAppSettings();
        //    ServiceResult result = null;

        //    try
        //    {
        //        var subscriptions = targetUserIds != null
        //            ? await GetSubscriptionsByUserIds(targetUserIds)
        //            : await GetAllSubscriptions();

        //        var vapidDetails = new VapidDetails("mailto:your-email@example.com", publicKey, privateKey);
        //        var webPushClient = new WebPushClient();
        //        int count = 0;

        //        foreach (var subscription in subscriptions)
        //        {
        //            var pushSubscription = new PushSubscription(subscription.EndPoint, subscription.P256dh, subscription.Auth);
        //            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
        //            await webPushClient.SendNotificationAsync(pushSubscription, jsonPayload, vapidDetails);
        //            count++;
        //        }

        //        if (subscriptions.Count == count)
        //        {
        //            result = new ServiceResult { StatusId = 1, RecordId = 1 };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //    }

        //    return result;
        //}

        public async Task<ServiceResult> SendPushNotificationAsync(PushPayLoad payload, IEnumerable<int>? targetUserIds = null)
        {
            var (publicKey, privateKey) = _vapidKeyService.GetKeysFromAppSettings();
            ServiceResult result = null;

            try
            {
                // Get subscriptions based on provided user IDs or all subscriptions
                var subscriptions = targetUserIds != null
                    ? await GetSubscriptionsByUserIds(targetUserIds)
                    : await GetAllSubscriptions();

                var vapidDetails = new VapidDetails("mailto:your-email@example.com", publicKey, privateKey);
                var webPushClient = new WebPushClient();
                int count = 0;

                // Iterate through each subscription and attempt to send notifications
                foreach (var subscription in subscriptions)
                {
                    try
                    {
                        var pushSubscription = new PushSubscription(subscription.EndPoint, subscription.P256dh, subscription.Auth);
                        var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);

                        // Try sending the notification for the current subscription
                        await webPushClient.SendNotificationAsync(pushSubscription, jsonPayload, vapidDetails);
                        count++; // Increment the count for successful notifications
                    }
                    catch (WebPushException webEx) when (webEx.Message.Contains("Subscription no longer valid"))
                    {
                        // Log and skip invalid subscriptions (unsubscribed or expired)
                        _logger.LogWarning($"Push subscription expired or unsubscribed for endpoint: {subscription.EndPoint}");
                        // Optionally, you can remove the expired subscription from the database
                        // await RemoveExpiredSubscription(subscription); // Implement this if needed
                    }
                    catch (Exception ex)
                    {
                        // Log any other unexpected errors and continue processing other subscriptions
                        _logger.LogError(ex, "Error sending push notification for subscription: " + subscription.EndPoint);
                    }
                }

                // Set the result if all subscriptions were processed successfully
                if (subscriptions.Count() == count)
                {
                    result = new ServiceResult { StatusId = 1, RecordId = 1 };
                }
                else
                {
                    result = new ServiceResult { StatusId = 0, RecordId = 0 };
                }
            }
            catch (Exception ex)
            {
                // Log the exception for any errors that occur in the overall process
                _logger.LogError(ex, "An error occurred while sending push notifications.");
                result = new ServiceResult { StatusId = 0, RecordId = 0 };
            }

            return result;
        }


        public async Task<int> SendPushNotificationAsync(string message, IEnumerable<int>? targetUserIds = null)
        {
            int pushCount = 0;

            try
            {
                var subscriptions = targetUserIds != null
                    ? await GetSubscriptionsByUserIds(targetUserIds)
                    : await GetAllSubscriptions();

                var vapidDetails = new VapidDetails
                {
                    Subject = "mailto:mail@example.com",
                    PublicKey = "BIFNpxaRNXajvNd9adipRuP0E3AJYMFIw60fJwSlZgiH_zEnCruLSoP5ywb8VZlrL2DgLGhZWBbWIzaiDL7SBWI",
                    PrivateKey = "aYXJDWWHDQJlgizT25Rjk2WVPyZytzAeCRGER0ao91E"
                };

                var webPushClient = new WebPushClient();
                int count = 0;

                foreach (var subscription in subscriptions)
                {
                    var pushSubscription = new PushSubscription(subscription.EndPoint, subscription.P256dh, subscription.Auth);
                    await webPushClient.SendNotificationAsync(pushSubscription, message, vapidDetails);
                    count++;
                }

                if (subscriptions.Count == count)
                {
                    pushCount = 1;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
            }

            return pushCount;
        }

        public async Task<List<pushsubscriptions>> GetSubscriptionsByUserIds(IEnumerable<int> targetUserIds)
        {
            try
            {
                var lst = await _context.pushsubscriptions
                .Where(m => targetUserIds.Contains(m.CreatedByUserId) && m.IsActive == ((int)ActiveState.Active).ToString())
                .ToListAsync();
                return lst;
            }
            catch (Exception ex)
            {
                return new List<pushsubscriptions>();
            }
            
            
        }

        public async Task<List<pushsubscriptions>> GetAllSubscriptions()
        {            
            try
            {
                var lst = await _context.pushsubscriptions
                .Where(m => m.IsActive == ((int)ActiveState.Active).ToString())
                .ToListAsync();
                return lst;
            }
            catch (Exception ex)
            {
                return new List<pushsubscriptions>();
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
