using MediatR;

namespace SC.SenseTower.Spaces.Cqrs.Commands
{
    public class SeedDataCommand : IRequest<Unit>
    {
        public string AccessToken { get; set; } = null!;
    }
}
