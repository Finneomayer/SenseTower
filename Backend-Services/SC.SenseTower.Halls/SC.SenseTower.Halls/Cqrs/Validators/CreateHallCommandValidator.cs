using FluentValidation;
using MongoDB.Driver;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Data;
using SC.SenseTower.Halls.Data.Models;

namespace SC.SenseTower.Halls.Cqrs.Validators
{
    public class CreateHallCommandValidator : AbstractValidator<CreateHallCommand>
    {
        public CreateHallCommandValidator(HallsDbContext context)
        {
            RuleFor(c => c.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указано название холла.")
                .MustAsync(async (name, cancellationToken) =>
                {
                    var filter = Builders<Hall>.Filter.Eq(r => r.Name, name);
                    return !await context.Halls.Find(filter).AnyAsync(cancellationToken);
                }).WithMessage("Холл с таким названием уже существует.");
            RuleFor(c => c.AccessToken)
                .NotEmpty().WithMessage("Вы не авторизованы, не можете добавлять данные");
        }
    }
}
