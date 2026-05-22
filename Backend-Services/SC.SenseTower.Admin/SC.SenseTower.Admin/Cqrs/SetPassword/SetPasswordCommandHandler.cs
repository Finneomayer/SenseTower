using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.SetPassword
{
    public class SetPasswordCommandHandler : BaseHandler, IRequestHandler<SetPasswordCommand, Unit>
    {
        private readonly IdentityService identityService;

        public SetPasswordCommandHandler(
            ILogger<SetPasswordCommandHandler> logger,
            IMapper mapper,
            IdentityService identityService) : base(logger, mapper)
        {
            this.identityService = identityService;
        }

#pragma warning disable CS8629 // Тип значения, допускающего NULL, может быть NULL.
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
        public async Task<Unit> Handle(SetPasswordCommand request, CancellationToken cancellationToken)
        {
            await identityService.SetPassword(request.UserId.Value, request.CurrentPassword, request.Password, cancellationToken);
            return Unit.Value;
        }
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
#pragma warning restore CS8629 // Тип значения, допускающего NULL, может быть NULL.
    }
}
