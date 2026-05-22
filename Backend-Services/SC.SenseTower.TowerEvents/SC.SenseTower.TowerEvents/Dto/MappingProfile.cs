using AutoMapper;
using SC.SenseTower.TowerEvents.Data.Models;
using SC.SenseTower.TowerEvents.Dto.Images;
using SC.SenseTower.TowerEvents.Dto.Spaces;
using SC.SenseTower.TowerEvents.Dto.TowerEvents;

namespace SC.SenseTower.TowerEvents.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TowerEvent, TowerEventListItemDto>()
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.Image == null ? null : s.Image.PreviewUrl))
                .ForMember(d => d.Sold, o => o.MapFrom(s => s.SoldTickets.Length))
                ;
            CreateMap<TowerEvent, TowerEventDto>()
                .ForMember(d => d.ImageId, o => o.MapFrom(s => s.Image == null ? (Guid?)null : s.Image.Id))
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.Image == null ? null : s.Image.PreviewUrl))
                .ForMember(d => d.Sold, o => o.MapFrom(s => s.SoldTickets.Length))
                ;
            CreateMap<Ticket, TicketDto>();
            CreateMap<UserInfo, UserInfoDto>().ReverseMap();
            CreateMap<ImageInfo, ImageInfoDto>().ReverseMap();
            CreateMap<LocalSpaceDto, Space>().ReverseMap();
            CreateMap<SpaceConnectionInfoDto, SpaceConnectionInfo>().ReverseMap();
            CreateMap<ImageInfoResponseDto, ImageInfo>();
        }
    }
}
