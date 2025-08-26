using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.DatCustomer;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using System.Linq.Dynamic.Core;

namespace NineERP.Application.Features.DatCustomerFeature.Queries;
public record GetDatCustomerPaginationQuery(DatCustomerRequest Request) : IRequest<PaginatedResult<DatCustomerDto>>;
public class GetDatCustomerPaginationQueryHandler : IRequestHandler<GetDatCustomerPaginationQuery, PaginatedResult<DatCustomerDto>>
{
    private readonly IApplicationDbContext _context;
    public GetDatCustomerPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<PaginatedResult<DatCustomerDto>> Handle(
        GetDatCustomerPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.DatCustomers.AsNoTracking()
            .Where(x => !x.IsDeleted &&
                (string.IsNullOrEmpty(request.Request.Keyword) ||
                x.CompanyName.ToLower().Contains(request.Request.Keyword.ToLower())));

        var totalRecords = await query.CountAsync(cancellationToken);

        var result = await query
            .OrderBy(request.Request.OrderBy)
            .Skip((request.Request.PageNumber - 1) * request.Request.PageSize)
            .Take(request.Request.PageSize)
            .Select(x => new DatCustomerDto
            {
                Id = x.Id,
                CompanyName = x.CompanyName,
                CustomerType = x.CustomerType,
                Website = x.Website,
                Email = x.Email,
                Address = x.Address,
                PhoneNo = x.PhoneNo,
                TaxNumber = x.TaxNumber,
                ProvinceId = x.ProvinceId,
                BankAccountName = x.BankAccountName,
                BankAccountNumber = x.BankAccountNumber,
                BankName = x.BankName,
                CountryId = x.CountryId,
                PostalCode = x.PostalCode,
                ImageUrl = x.ImageUrl,
                Status = x.Status                
            })
            .ToListAsync(cancellationToken);

        return PaginatedResult<DatCustomerDto>.Success(result, totalRecords, request.Request.PageNumber, request.Request.PageSize);
    }
}
