using AutoMapper;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Data.Models;
using SC.SenseTower.Accounts.Dto.Invites;
using SC.SenseTower.Accounts.Dto.UserInfo;
using SC.SenseTower.Accounts.Dto.Wallets;

namespace SC.SenseTower.Accounts.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AddWalletCommand, Wallet>()
                .ForMember(r => r.Id, o => o.MapFrom(s => s.WalletId))
                .ForMember(r => r.IsConfirmed, o => o.MapFrom(s => false))
                .ForMember(r => r.IsActive, o => o.MapFrom(s => false));
            CreateMap<Wallet, WalletItemDto>();
            CreateMap<Wallet, WalletDto>();

            CreateMap<UsingInfo, UsingInfoDto>();
            CreateMap<RecallInfo, RecallInfoDto>();
            CreateMap<Invite, UserInviteDto>();
        }
    }
}
