using AutoMapper;
using Bank.Application.DTOs.Statement.Core;
using Bank.Application.DTOs.Statement.Search;
using Bank.Application.DTOs.Statement.Summary;
using Bank.Application.DTOs.Statement.Delivery;
using Bank.Application.DTOs.Statement.Analytics;
using Bank.Application.DTOs.Statement.Transaction;
using Bank.Domain.Entities;

namespace Bank.Application.Mappings.Statement
{
    /// <summary>
    /// AutoMapper profile for AccountStatement entity mappings
    /// </summary>
    public class StatementMappingProfile : Profile
    {
        public StatementMappingProfile()
        {
            CreateMap<AccountStatement, StatementDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.PeriodStartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.PeriodEndDate))
                .ForMember(dest => dest.OpeningBalance, opt => opt.MapFrom(src => src.OpeningBalance))
                .ForMember(dest => dest.ClosingBalance, opt => opt.MapFrom(src => src.ClosingBalance))
                .ReverseMap();

            CreateMap<AccountStatement, GenerateStatementRequest>().ReverseMap();
        }
    }
}
