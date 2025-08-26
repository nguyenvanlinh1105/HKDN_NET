using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace NineERP.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class LanguageController : Controller
    {
        public IActionResult SetLanguage(string culture, string returnUrl = "/")
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true, // 🟢 Cực kỳ quan trọng nếu CookiePolicy đang bật
                    SameSite = SameSiteMode.Lax
                }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
