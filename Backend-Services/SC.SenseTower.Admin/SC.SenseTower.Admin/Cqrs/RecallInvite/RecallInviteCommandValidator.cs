using FluentValidation;
using SC.SenseTower.Admin.Data.Models.Accounts;
using SC.SenseTower.Admin.Services;

namespace SC.SenseTower.Admin.Cqrs.RecallInvite
{
    public class RecallInviteCommandValidator : AbstractValidator<RecallInviteCommand>
    {
        private Invite? invite = null;

        public RecallInviteCommandValidator(InvitesService invitesService)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан код приглашения.")
                .MustAsync(async (c, inviteId, cancellationToken) =>
                {
                    invite ??= await invitesService.Get(inviteId, cancellationToken);
                    return invite != null;
                }).WithMessage("Приглашение с таким кодом не существует.")
                .MustAsync(async (c, inviteId, cancellationToken) =>
                {
                    invite ??= await invitesService.Get(inviteId, cancellationToken);
                    return invite.UsingInfo?.Date == null;
                }).WithMessage("Приглашение уже использовано, невозможно отозвать.")
                .MustAsync(async (c, inviteId, cancellationToken) =>
                {
                    invite ??= await invitesService.Get(inviteId, cancellationToken);
                    return !(invite.RecallInfo?.IsRecalled ?? false);
                }).WithMessage("Приглашение уже отозвано, невозможно отозвать повторно.");
        }
    }
}
