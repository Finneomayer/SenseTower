using FluentValidation;
using MongoDB.Driver;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Data;
using SC.SenseTower.Halls.Data.Models;

namespace SC.SenseTower.Halls.Cqrs.Validators
{
    public class UpdateHallCommandValidator : AbstractValidator<UpdateHallCommand>
    {
        private Hall? hall = null;

        public UpdateHallCommandValidator(HallsDbContext context)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор холла.")
                .MustAsync(async (hallId, cancellationToken) =>
                {
                    hall ??= await context.Halls.Find(x => x.Id == hallId).FirstOrDefaultAsync(cancellationToken);
                    return hall != null;
                }).WithMessage("Холл не найден.");
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указано название холла.")
                .MustAsync(async (c, name, cancellationToken) =>
                {
                    if (hall != null && hall.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                    var filter = Builders<Hall>.Filter.Eq(r => r.Name, name);
                    filter &= Builders<Hall>.Filter.Ne(r => r.Id, c.Id);
                    return !await context.Halls.Find(filter).AnyAsync(cancellationToken);
                }).WithMessage("Холл с таким названием уже существует.");
        }
    }
}
