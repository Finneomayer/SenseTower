using FluentValidation;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Data.Models;
using SC.SenseTower.Accounts.Services;

namespace SC.SenseTower.Accounts.Validators.Identity
{
    public class RegisterDataValidator : AbstractValidator<RegisterUserCommand>
    {
        private Invite? invite = null;

        public RegisterDataValidator(IdentityService identityService, InvitesService invitesService)
        {
            RuleFor(x => x.InviteId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан код приглашения")
                .MustAsync(async (inviteId, cancellationToken) =>
                {
                    invite ??= await invitesService.Get(inviteId, cancellationToken);
                    return invite != null;
                }).WithMessage("Приглашение с указанным кодом не существует")
                .MustAsync(async (inviteId, canellationToken) =>
                {
                    invite ??= await invitesService.Get(inviteId, canellationToken);
                    return invite?.UsingInfo.UserId == null;
                }).WithMessage("Приглашение с указанным кодом уже использовано")
                .MustAsync(async (inviteId, cancellationToken) =>
                {
                    invite ??= await invitesService.Get(inviteId, cancellationToken);
                    return !(invite?.RecallInfo.IsRecalled ?? false);
                }).WithMessage("Приглашение с указанным кодом отозвано и больше недействительно");
            RuleFor(x => x.Login)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указано имя входа")
                .MinimumLength(3).WithMessage("Имя входа не может быть короче 3 символов")
                .MaximumLength(30).WithMessage("Имя входа не может быть длиннее 30 символов")
                .MustAsync(async (val, _) =>
                {
                    return await identityService.IsLoginFree(val, default);
                }).WithMessage("Пользователь с таким именем входа уже зарегистрирован");
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Пароль не указан")
                .MinimumLength(12).WithMessage("Пароль не может быть короче 12 символов")
                .MaximumLength(30).WithMessage("Пароль не может быть длиннее 30 символов");
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан email")
                .EmailAddress().WithMessage("Неверный формат email")
                .Must(val => val.Substring(val.IndexOf('@') + 1).IndexOf('.') > 0).WithMessage("Неверный формат email")
                .MinimumLength(5).WithMessage("Email не может быть короче 5 символов")
                .MaximumLength(100).WithMessage("Email не может быть длиннее 100 символов")
                .MustAsync(async (val, _) =>
                {
                    return await identityService.IsEmailFree(val, default);
                }).WithMessage("Пользователь с таким email уже зарегистрирован");
        }
    }
}
