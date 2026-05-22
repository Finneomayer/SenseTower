using FluentValidation;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Data.Models;
using SC.SenseTower.Accounts.Services;

namespace SC.SenseTower.Accounts.Validators.Identity
{
    public class RegisterUserByTicketValidator : AbstractValidator<RegisterUserByTicketCommand>
    {
        private Ticket? ticket = null;

        public RegisterUserByTicketValidator(IdentityService identityService, TicketsService ticketsService)
        {
            RuleFor(x => x.TicketId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан код билета")
                .MustAsync(async (ticketId, cancellationToken) =>
                {
                    ticket ??= await ticketsService.Get(ticketId, cancellationToken);
                    return ticket != null;
                }).WithMessage("Билет с указанным кодом не существует")
                .MustAsync(async (ticketId, canellationToken) =>
                {
                    ticket ??= await ticketsService.Get(ticketId, canellationToken);
                    return ticket?.UsingInfo.UserId == null;
                }).WithMessage("Билет с указанным кодом уже использован")
                .MustAsync(async (ticketId, cancellationToken) =>
                {
                    ticket ??= await ticketsService.Get(ticketId, cancellationToken);
                    return !(ticket?.RecallInfo.IsRecalled ?? false);
                }).WithMessage("Билет с указанным кодом отозван и больше недействителен");
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
