using FluentValidation;
using SC.SenseTower.Accounts.Constants;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Data.Models;
using SC.SenseTower.Accounts.Services;

namespace SC.SenseTower.Accounts.Validators.Invites
{
    public class RecallInviteCommandValidator : AbstractValidator<RecallInviteCommand>
    {
        private Invite? invite = null;

        public RecallInviteCommandValidator(InvitesService invitesService)
        {
            RuleFor(x => x.InviteId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан код приглашения.")
                .MustAsync(async (inviteId, cancellationToken) =>
                {
                    invite ??= await invitesService.Get(inviteId ?? string.Empty, cancellationToken);
                    return invite != null;
                }).WithMessage("Приглашение не найдено.");
            RuleFor(x => x.Role).NotEmpty().WithMessage("Права пользователя не определены.");
            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Пользователь не авторизован.")
                .MustAsync(async (c, userId, cancellationToken) =>
                {
                    invite ??= await invitesService.Get(c.InviteId ?? string.Empty, cancellationToken);
                    return invite?.IssuerId == userId || c.Role == RoleNames.VR_ADMIN;
                }).WithMessage("Нельзя отозвать чужое приглашение.");
        }
    }
}
