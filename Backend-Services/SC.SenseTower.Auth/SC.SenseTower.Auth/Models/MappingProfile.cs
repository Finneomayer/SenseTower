using AutoMapper;

namespace SC.SenseTower.Auth.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, LookupItemDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.UserName));
        }
    }
}
