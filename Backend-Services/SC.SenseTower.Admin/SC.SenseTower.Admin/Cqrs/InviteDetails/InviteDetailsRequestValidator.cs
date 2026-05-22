using FluentValidation;
using SC.SenseTower.Admin.Services;

namespace SC.SenseTower.Admin.Cqrs.InviteDetails
{
    public class InviteDetailsRequestValidator : AbstractValidator<InviteDetailsRequest>
    {
        public InviteDetailsRequestValidator(InvitesService invitesService)
        {
            RuleFor(x => x.InviteId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан код приглашения.")
                .MustAsync(async (c, inviteId, cancellationToken) =>
                {
                    var invite = await invitesService.Get(inviteId, cancellationToken);
                    return invite != null;
                }).WithMessage("Приглашения с таким кодом не существует.");
        }
    }
}
