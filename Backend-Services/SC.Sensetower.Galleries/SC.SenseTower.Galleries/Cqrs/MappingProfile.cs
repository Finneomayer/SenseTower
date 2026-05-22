using AutoMapper;
using SC.SenseTower.Galleries.Cqrs.CreateGallery;
using SC.SenseTower.Galleries.Cqrs.CreateGalleryImage;
using SC.SenseTower.Galleries.Data.Models;

namespace SC.SenseTower.Galleries.Cqrs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateGalleryCommand, Data.Models.Gallery>()
                .ForPath(d => d.GalleryInfoTable.IsVisible, o => o.MapFrom(s => s.IsVisible))
                .ForPath(d => d.GalleryInfoTable.Description, o => o.MapFrom(s => s.Description))
                ;
            CreateMap<CreateGalleryImageCommand, GalleryImage>()
                .ForPath(d => d.Image.Id, o => o.MapFrom(s => s.ImageId))
                ;
            CreateMap<CreateGalleryImageCommand, Picture>()
                .ForMember(d => d.Image, o => o.MapFrom(s => s))
                ;
        }
    }
}
