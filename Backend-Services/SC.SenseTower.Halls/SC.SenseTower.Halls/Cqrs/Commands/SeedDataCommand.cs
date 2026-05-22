using MediatR;

namespace SC.SenseTower.Halls.Cqrs.Commands
{
    public class SeedDataCommand : IRequest<Unit>
    {
        public string AccessToken { get; set; } = null!;
    }
}
