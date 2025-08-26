using System.Text.Json;
using Amazon.SimpleNotificationService.Model;
using Amazon.SimpleNotificationService;
using Microsoft.Extensions.Configuration;
using NineERP.Application.Interfaces.AwsNotification;
using Microsoft.Extensions.Logging;
using NineERP.Application.Dtos.AwsNotification;

namespace NineERP.Infrastructure.Services.AwsNotification
{
    public class AwsNotificationService(
    IAmazonSimpleNotificationService snsClient,
    ILogger<AwsNotificationService> logger,
    IConfiguration configuration
) : INotificationService
    {
        public async Task<AwsNotificationDto> RegisterDeviceTokenAsync(string platform, string deviceToken, List<string>? topics = null)
        {
            logger.LogInformation("➡️ [RegisterDeviceTokenAsync] Start. Platform: {Platform}, DeviceToken: {Token}, TopicsCount: {Count}", platform, deviceToken, topics?.Count ?? 0);

            try
            {
                var platformApplicationArn = GetPlatformApplicationArn(platform);
                logger.LogDebug("✅ PlatformApplicationArn: {PlatformArn}", platformApplicationArn);

                var request = new CreatePlatformEndpointRequest
                {
                    PlatformApplicationArn = platformApplicationArn,
                    Token = deviceToken
                };

                var response = await snsClient.CreatePlatformEndpointAsync(request);
                logger.LogInformation("✅ Endpoint registered: {EndpointArn}", response.EndpointArn);

                var endpointTopicsArn = new List<string>();

                if (topics is { Count: > 0 })
                {
                    logger.LogDebug("🔁 Subscribing endpoint to {TopicCount} topic(s)...", topics.Count);
                    foreach (var topic in topics)
                    {
                        var topicArn = GetTopicsArn(topic);
                        logger.LogDebug("➡️ Subscribing to topic: {Topic}, Arn: {Arn}", topic, topicArn);

                        var subscribeRequest = new SubscribeRequest
                        {
                            TopicArn = topicArn,
                            Protocol = "application",
                            Endpoint = response.EndpointArn
                        };

                        var subscribeResponse = await snsClient.SubscribeAsync(subscribeRequest);
                        logger.LogInformation("✅ Subscribed to topic {Topic} with SubscriptionArn: {SubscriptionArn}", topic, subscribeResponse.SubscriptionArn);

                        endpointTopicsArn.Add(subscribeResponse.SubscriptionArn);
                    }
                }

                logger.LogInformation("✅ [RegisterDeviceTokenAsync] Completed successfully.");

                return new AwsNotificationDto
                {
                    EndpointDeviceTokenArn = response.EndpointArn,
                    EndpointTopicsArn = endpointTopicsArn
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ [RegisterDeviceTokenAsync] Error occurred while registering device token.");
                throw;
            }
        }

        public async Task SendPushNotificationAsync(string endpointArn, string title, string body)
        {
            logger.LogInformation("➡️ [SendPushNotificationAsync] Start. EndpointArn: {EndpointArn}, Title: {Title}", endpointArn, title);

            try
            {
                var payload = new
                {
                    notification = new
                    {
                        title,
                        body
                    }
                };

                var jsonMessage = JsonSerializer.Serialize(new
                {
                    body,
                    GCM = JsonSerializer.Serialize(payload)
                });

                logger.LogDebug("📦 Payload to send: {Payload}", jsonMessage);

                var request = new PublishRequest
                {
                    TargetArn = endpointArn,
                    MessageStructure = "json",
                    Message = jsonMessage
                };

                var response = await snsClient.PublishAsync(request);
                logger.LogInformation("✅ Notification sent to endpoint {EndpointArn}. MessageId: {MessageId}", endpointArn, response.MessageId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ [SendPushNotificationAsync] Failed to send notification to endpoint: {EndpointArn}", endpointArn);
                throw;
            }
        }

        private string GetPlatformApplicationArn(string platform)
        {
            var lower = platform.ToLower();
            var arn = lower switch
            {
                "android" => configuration["AWS:SNS:AndroidPlatformArn"],
                "ios" => configuration["AWS:SNS:IOSPlatformArn"],
                "web" => configuration["AWS:SNS:WebPlatformArn"],
                _ => throw new ArgumentException($"❌ Unsupported platform: {platform}")
            };

            if (string.IsNullOrWhiteSpace(arn)) logger.LogWarning("⚠️ ARN not found in config for platform: {Platform}", platform);

            return arn!;
        }

        private string GetTopicsArn(string topic)
        {
            var arn = configuration[$"AWS:TOPICS:{topic}"];
            if (string.IsNullOrWhiteSpace(arn)) logger.LogWarning("⚠️ Topic ARN not found in config for topic: {Topic}", topic);
            return arn!;
        }
    }
}
