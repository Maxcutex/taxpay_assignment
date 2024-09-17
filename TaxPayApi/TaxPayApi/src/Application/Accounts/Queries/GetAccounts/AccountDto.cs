using AutoMapper;
using TaxPayApi.Application.Common.Mappings;

namespace TaxPayApi.Application.Accounts.Queries.GetAccounts;

public class AccountDto : IMapFrom<Domain.Entities.Account>
{
    public int Id { get; set; }
    
    public string AccountId { get; private set; }

    public string Title { get; private set; }

    public decimal Balance { get; private set; }
    public string Type { get; private set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Entities.Account, AccountDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));
    }
}