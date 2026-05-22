using MediatR;
using SC.SenseTower.Admin.Dto.Invites;

namespace SC.SenseTower.Admin.Cqrs.InviteDetails
{
    public class InviteDetailsRequest : IRequest<InviteDetailsDto>
    {
        public string InviteId { get; set; } = null!;
    }
}
