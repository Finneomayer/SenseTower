using MediatR;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Admin.Dto.Users;

namespace SC.SenseTower.Admin.Cqrs.UserDetails
{
    public class UserDetailsRequest : ExternalRequestDto, IRequest<UserDetailsDto>
    {
        public Guid UserId { get; set; }
    }
}
