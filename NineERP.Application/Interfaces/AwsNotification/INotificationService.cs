using NineERP.Application.Dtos.AwsNotification;

namespace NineERP.Application.Interfaces.AwsNotification
{
    public interface INotificationService
    {
        Task<AwsNotificationDto> RegisterDeviceTokenAsync(string platform, string deviceToken, List<string>? topics = null);
        Task SendPushNotificationAsync(string endpointArn, string title, string body);
    }
}
