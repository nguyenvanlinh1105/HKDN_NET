namespace NineERP.Application.Dtos.AwsNotification
{
    public class AwsNotificationDto
    {
        public string? EndpointDeviceTokenArn { get; set; }
        public List<string>? EndpointTopicsArn { get; set; }
    }
}
