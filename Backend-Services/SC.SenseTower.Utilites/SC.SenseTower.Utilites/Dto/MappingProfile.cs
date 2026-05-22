using AutoMapper;
using SC.SenseTower.Utilities.Dto.YandexVm;

namespace SC.SenseTower.Utilities.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<InstanceDto, UpdateInstanceDto>();
        }
    }
}
