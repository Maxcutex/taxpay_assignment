using TaxPayApi.Application.Common.Interfaces;

namespace TaxPayApi.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}