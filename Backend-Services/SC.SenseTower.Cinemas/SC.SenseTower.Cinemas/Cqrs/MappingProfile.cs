using AutoMapper;
using SC.SenseTower.Cinemas.Cqrs.CinemaCreate;
using SC.SenseTower.Cinemas.Cqrs.CinemaUpdate;

namespace SC.SenseTower.Cinemas.Cqrs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CinemaCreateCommand, Data.Models.Cinema>();
            CreateMap<CinemaUpdateCommand, Data.Models.Cinema>();
        }
    }
}
