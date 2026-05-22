using AutoMapper;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Data.Models;

namespace SC.SenseTower.Halls.Cqrs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateHallCommand, Hall>();
            CreateMap<UpdateHallCommand, Hall>();

            CreateMap<Hall, LookupItemDto<Guid>>();
        }
    }
}
