using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using TaxPayApi.Application.Accounts.Queries.GetAccounts;
using TaxPayApi.Application.Common.Interfaces;
using TaxPayApi.Application.Common.Mappings;
using TaxPayApi.Application.Common.Models;
using TaxPayApi.Domain.Enums;

namespace TaxPayApi.Application.Accounts.Queries.GetAccountsWithPagination;

public record GetAccountsWithPaginationQuery : IRequest<PaginatedList<AccountDto>>
{
    public AccountType? Type { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class
    GetAccountsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetAccountsWithPaginationQuery,
        PaginatedList<AccountDto>>
{
    public async Task<PaginatedList<AccountDto>> Handle(GetAccountsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Accounts
            .Where(x => request.Type == null || x.Type == request.Type)
            .OrderBy(x => x.Title)
            .ProjectTo<AccountDto>(mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}