using AutoMapper;
using IdentityModel.Client;
using SC.SenseTower.Accounts.Dto.Identity;
using SC.SenseTower.Accounts.Dto.UserInfo;

namespace SC.SenseTower.Accounts.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TokenResponse, TokenResponseDto>();
            CreateMap<TokenResponse, ClientLogonResultDto>();
            CreateMap<TokenResponse, RefreshUserTokenResultDto>();
            CreateMap<UserInfoResponseDto, LogonResultDto>();
            CreateMap<UserInfoResponseDto, UserInfoDto>();
        }
    }
}
