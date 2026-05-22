using AutoMapper;
using SC.SenseTower.Galleries.Data.Models;
using SC.SenseTower.Galleries.Dto.Galleries;
using SC.SenseTower.Galleries.Dto.Images;
using SC.SenseTower.Galleries.Dto.Spaces;

namespace SC.SenseTower.Galleries.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SpaceConnectionInfo, ConnectionInfoDto>().ReverseMap();
            CreateMap<ImageInfo, ImageInfoDto>().ReverseMap();
            CreateMap<UserInfo, UserInfoDto>();
            CreateMap<Space, SpaceDto>().ReverseMap();

            CreateMap<ImageInfo, ImageInfoDto>().ReverseMap();
            CreateMap<GalleryImage, GalleryImageDto>();
            CreateMap<GalleryInfoTable, InfoTableDto>()
                .ForMember(d => d.ShowInformation, o => o.MapFrom(s => s.IsVisible))
                ;
            CreateMap<Gallery, GalleryDto>()
                .ForMember(d => d.PicturesLocation, o => o.MapFrom(s => s.Pictures.Select(x => new KeyValuePair<int, GalleryImageDto>(x.Position, new GalleryImageDto
                {
                    Author = x.Image.Author,
                    Description = x.Image.Description,
                    Image = new ImageInfoDto
                    {
                        FileUrl = x.Image.Image.FileUrl,
                        Id = x.Image.Image.Id,
                        Name = x.Image.Image.Name,
                        PreviewUrl = x.Image.Image.PreviewUrl
                    },
                    Name = x.Image.Name,
                    PassepartoutWidthInMeters = x.Image.PassepartoutWidthInMeters,
                    PictureWidthInMeters = x.Image.PictureWidthInMeters
                }))))
                ;
            CreateMap<Gallery, GalleryItemDto>()
                .ForMember(d => d.InfoTable, o => o.MapFrom(s => s.GalleryInfoTable))
                .ForMember(d => d.PicturesCounter, o => o.MapFrom(s => s.Pictures.Count()))
                ;
        }
    }
}
