using AutoMapper;
using Bank.Application.DTOs;
using Bank.Application.DTOs.Account.Core;
using Bank.Application.DTOs.Account.JointAccount;
using Bank.Domain.Entities;

namespace Bank.Application.Mappings.Account
{
    /// <summary>
    /// AutoMapper profile for JointAccount entity mappings
    /// </summary>
    public class JointAccountMappingProfile : Profile
    {
        public JointAccountMappingProfile()
        {
            CreateMap<Bank.Domain.Entities.JointAccount, JointAccountDto>().ReverseMap();
            CreateMap<Bank.Domain.Entities.JointAccount, CreateJointAccountRequest>().ReverseMap();
            CreateMap<Bank.Domain.Entities.JointAccountHolder, JointAccountHolderDetailsDto>().ReverseMap();
        }
    }
}
