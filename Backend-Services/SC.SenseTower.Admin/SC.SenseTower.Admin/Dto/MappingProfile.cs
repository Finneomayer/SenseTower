using AutoMapper;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Rendering;
using SC.SenseTower.Admin.Cqrs.GalleryCreate;
using SC.SenseTower.Admin.Cqrs.GalleryImageAdd;
using SC.SenseTower.Admin.Cqrs.GalleryUpdate;
using SC.SenseTower.Admin.Cqrs.PlaceCreate;
using SC.SenseTower.Admin.Cqrs.PlaceUpdate;
using SC.SenseTower.Admin.Cqrs.TowerEventCreate;
using SC.SenseTower.Admin.Cqrs.TowerEventUpdate;
using SC.SenseTower.Admin.Data.Models.Accounts;
using SC.SenseTower.Admin.Data.Models.Identity;
using SC.SenseTower.Admin.Dto.Galleries;
using SC.SenseTower.Admin.Dto.Identity;
using SC.SenseTower.Admin.Dto.Invites;
using SC.SenseTower.Admin.Dto.Places;
using SC.SenseTower.Admin.Dto.Tickets;
using SC.SenseTower.Admin.Dto.TowerEvents;
using SC.SenseTower.Admin.Dto.Users;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, LookupItemDto<Guid>>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.UserName));
            CreateMap<ApplicationUser, UserGridItemDto>()
                .ForMember(d => d.IsActive, o => o.MapFrom(s => false))
                .ForMember(d => d.IsLocked, o => o.MapFrom(s => s.LockoutEnd > DateTime.UtcNow));
            CreateMap<ApplicationUser, UserDetailsDto>()
                .ForMember(d => d.LockoutEnd, o => o.MapFrom(s => s.LockoutEnd != null ? new DateTime(s.LockoutEnd.Value.UtcTicks) : (DateTime?)null))
                .ForMember(d => d.RoleId, o => o.MapFrom(s => s.Roles.Count == 0 ? (Guid?)null : s.Roles[0]))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedOn));
            CreateMap<ApplicationRole, LookupItemDto<Guid>>();

            CreateMap<LookupItemDto<Guid>, SelectListItem>()
                .ForMember(d => d.Value, o => o.MapFrom(s => s.Id.ToString()))
                .ForMember(d => d.Text, o => o.MapFrom(s => s.Name));

            CreateMap<Invite, InviteGridItemDto>()
                .ForMember(d => d.UsedAt, o => o.MapFrom(s => s.UsingInfo.Date))
                .ForMember(d => d.IsRecalled, o => o.MapFrom(s => s.RecallInfo.Date != null));
            CreateMap<Invite, InviteDetailsDto>()
                .ForMember(d => d.RecallDate, o => o.MapFrom(s => s.RecallInfo.Date))
                .ForMember(d => d.RecallReason, o => o.MapFrom(s => s.RecallInfo.RecallReason))
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.UsingInfo.UserId))
                .ForMember(d => d.UsingDate, o => o.MapFrom(s => s.UsingInfo.Date));
            CreateMap<Invite, UserInviteListItemDto>()
                .ForMember(d => d.IsRecalled, o => o.MapFrom(s => s.RecallInfo != null && s.RecallInfo.IsRecalled));

            CreateMap<Ticket, TicketGridItemDto>()
                .ForMember(d => d.UsedAt, o => o.MapFrom(s => s.UsingInfo.Date))
                .ForMember(d => d.IsRecalled, o => o.MapFrom(s => s.RecallInfo.Date != null));
            CreateMap<Ticket, TicketDetailsDto>()
                .ForMember(d => d.RecallDate, o => o.MapFrom(s => s.RecallInfo.Date))
                .ForMember(d => d.RecallReason, o => o.MapFrom(s => s.RecallInfo.RecallReason))
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.UsingInfo.UserId))
                .ForMember(d => d.UsingDate, o => o.MapFrom(s => s.UsingInfo.Date));

            CreateMap<Wallet, UserWalletListItemDto>();

            CreateMap<TokenResponse, RefreshTokenResultDto>();

            CreateMap<GalleryItemResponseDto, GalleryGridItemDto>()
                .ForMember(d => d.IsVisible, o => o.MapFrom(s => s.InfoTable.ShowInformation))
                .ForMember(d => d.SceneName, o => o.MapFrom(s => s.Space.SceneName))
                .ForMember(d => d.PicturesCount, o => o.MapFrom(s => s.PicturesCounter))
                ;
            CreateMap<PagedDataDto<GalleryItemResponseDto>, PagedDataDto<GalleryGridItemDto>>();
            CreateMap<GalleryResponseDto, GalleryDto>()
                .ForMember(d => d.Description, o => o.MapFrom(s => s.GalleryInfoTable.Description))
                .ForMember(d => d.ImageId, o => o.MapFrom(s => s.GalleryInfoTable.Image.Id))
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.GalleryInfoTable.Image.PreviewUrl))
                .ForMember(d => d.IsVisible, o => o.MapFrom(s => s.GalleryInfoTable.ShowInformation))
                .ForMember(d => d.SpaceId, o => o.MapFrom(s => s.Space.Id))
                ;
            CreateMap<GalleryCreateCommand, GalleryCreateRequestDto>()
                .ForMember(d => d.SpaceId, o => o.MapFrom(s => s.SpaceId == null ? Guid.Empty : s.SpaceId.Value))
                ;
            CreateMap<GalleryUpdateCommand, GalleryUpdateRequestDto>()
                .ForMember(d => d.SpaceId, o => o.MapFrom(s => s.SpaceId == null ? Guid.Empty : s.SpaceId.Value))
                ;
            CreateMap<GalleryImageResponseDto, GalleryImageDto>()
                .ForMember(d => d.ImageId, o => o.MapFrom(s => s.Image.Id))
                .ForMember(d => d.ImageName, o => o.MapFrom(s => s.Image.Name))
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.Image.PreviewUrl))
                ;
            CreateMap<GalleryImageAddCommand, GalleryAddImageDto>();

            CreateMap<TowerEventItemResponseDto, TowerEventGridItemDto>()
                .ForMember(d => d.SpaceName, o => o.MapFrom(s => s.Space == null ? string.Empty : s.Space.SpaceName))
                .ForMember(d => d.Sold, o => o.MapFrom(s => s.SoldTickets.Length))
                ;
            CreateMap<TowerEventResponseDto, TowerEventDto>()
                .ForMember(d => d.SpaceId, o => o.MapFrom(s => s.Space == null ? (Guid?)null : s.Space.Id))
                ;
            CreateMap<TowerEventUpdateCommand, TowerEventUpdateRequestDto>()
                .ForMember(d => d.Date, o => o.MapFrom(s => s.From))
                ;
            CreateMap<TowerEventCreateCommand, TowerEventCreateRequestDto>()
                .ForMember(d => d.Date, o => o.MapFrom(s => s.From))
                .ForMember(d => d.TicketQuantity, o => o.MapFrom(s => s.TotalTickets))
                ;

            CreateMap<PlacesPageItemResponseDto, PlacesGridItemDto>()
                .ForMember(d => d.DoorImageUrl, o => o.MapFrom(s => s.DoorImage.PreviewUrl))
                .ForMember(d => d.ImageCount, o => o.MapFrom(s => s.Images != null ? s.Images.Count : 0))
                .ForMember(d => d.SpaceId, o => o.MapFrom(s => s.LocalSpace != null ? s.LocalSpace.Id : (Guid?)null))
                .ForMember(d => d.SpaceName, o => o.MapFrom(s => s.LocalSpace != null ? s.LocalSpace.Name : null))
                ;
            CreateMap<PlaceResponseDto, PlaceDto>()
                .ForMember(d => d.DoorImageId, o => o.MapFrom(s => s.DoorImage.Id == default ? (Guid?)null : s.DoorImage.Id))
                .ForMember(d => d.DoorImageUrl, o => o.MapFrom(s => s.DoorImage.PreviewUrl))
                .ForMember(d => d.SpaceId, o => o.MapFrom(s => s.LocalSpace == null ? (Guid?)null : s.LocalSpace.Id))
                ;
            CreateMap<PlaceUpdateCommand, PlaceSaveDto>()
                .ForMember(d => d.PlaceNumber, o => o.MapFrom(s => s.Number))
                ;
            CreateMap<PlaceCreateCommand, PlaceSaveDto>();
        }
    }
}
