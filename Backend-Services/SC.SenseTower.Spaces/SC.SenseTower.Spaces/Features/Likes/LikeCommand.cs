using MediatR;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Spaces.Features.Likes;

public record LikeCommand : IRequest<Unit>
{
    /// <summary>
    /// Идентификатор пространства.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Лайк, дизлайк или отмена оценки
    /// </summary>
    public bool? Like { get; set; }

    /// <summary>
    /// ID пользователя
    /// </summary>
    public Guid UserId { get; set; }
}