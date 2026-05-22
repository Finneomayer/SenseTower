using MediatR;

namespace SC.SenseTower.Accounts.Cqrs.Commands
{
    public class DeleteUserPlacesCommand : IRequest<bool>
    {
        public Guid? OwnerId { get; set; }

        public string? AccessToken { get; set; }
    }
}
