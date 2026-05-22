using FluentValidation;
using MongoDB.Driver;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Data;
using SC.SenseTower.Halls.Data.Models;

namespace SC.SenseTower.Halls.Cqrs.Validators
{
    public class DeleteHallCommandValidator : AbstractValidator<DeleteHallCommand>
    {
        public DeleteHallCommandValidator(HallsDbContext context)
        {
            RuleFor(x => x.HallId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор холла.")
                .MustAsync(async (hallId, cancellationToken) =>
                {
                    var filter = Builders<Hall>.Filter.Eq(x => x.Id, hallId);
                    return await context.Halls.Find(filter).AnyAsync(cancellationToken);
                }).WithMessage("Холл с таким идентификатором не найден.");
        }
    }
}
