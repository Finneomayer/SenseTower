using AutoMapper;
using SC.SenseTower.Images.Constants;
using SC.SenseTower.Images.Data.Models;
using SC.SenseTower.Images.Dto.Images;

namespace SC.SenseTower.Images.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ImageFile, ImageDto>()
                .ForMember(d => d.FileUrl, o => o.MapFrom(s => ApiConstants.API_ROOT_SEGMENT + "images/download/" + s.Id.ToString() + "?preview=false"))
                .ForMember(d => d.PreviewUrl, o => o.MapFrom(s => ApiConstants.API_ROOT_SEGMENT + "images/download/" + s.Id.ToString() + "?preview=true"));
            CreateMap<ImageFile, ImageListItemDto>()
                .ForMember(d => d.FileUrl, o => o.MapFrom(s => ApiConstants.API_ROOT_SEGMENT + "images/download/" + s.Id.ToString() + "?preview=false"))
                .ForMember(d => d.PreviewUrl, o => o.MapFrom(s => ApiConstants.API_ROOT_SEGMENT + "images/download/" + s.Id.ToString() + "?preview=true"));
            CreateMap<ImageFile, ImageInfoDto>()
                .ForMember(d => d.FileUrl, o => o.MapFrom(s => ApiConstants.API_ROOT_SEGMENT + "images/download/" + s.Id.ToString() + "?preview=false"))
                .ForMember(d => d.PreviewUrl, o => o.MapFrom(s => ApiConstants.API_ROOT_SEGMENT + "images/download/" + s.Id.ToString() + "?preview=true"));
        }
    }
}
