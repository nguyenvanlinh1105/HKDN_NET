using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using NineERP.Application.Interfaces.AwsNotification;

namespace NineERP.Web.Controllers.Api
{
    [ApiController]
    [Route("api/notification")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class NotificationsController(INotificationService notificationService) : ControllerBase
    {
        [HttpPost("register-device")]
        public async Task<IActionResult> Register([FromBody] string deviceToken)
        {
            string platform = "web";
            var endpointArn = await notificationService.RegisterDeviceTokenAsync(platform, deviceToken);

            return Ok(new { endpointArn });
        }

        [HttpPost("send-notification")]
        public async Task<IActionResult> SendPush([FromBody] string endpointArn)
        {
            await notificationService.SendPushNotificationAsync(endpointArn, "Xin chào", "Thông báo từ AWS SNS");

            return Ok("Push sent");
        }
    }
}
