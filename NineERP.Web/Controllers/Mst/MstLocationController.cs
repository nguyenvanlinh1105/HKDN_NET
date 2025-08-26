using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Features.MstLocationFeature.Queries;

namespace NineERP.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class MstLocationController(IMediator mediator) : BaseController
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetCountries()
    {
        var result = await mediator.Send(new GetAllMstCountryQuery());
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetProvincesByCountry(short countryId)
    {
        var result = await mediator.Send(new GetProvincesByCountryIdQuery(countryId));
        return Ok(result);
    }
}
