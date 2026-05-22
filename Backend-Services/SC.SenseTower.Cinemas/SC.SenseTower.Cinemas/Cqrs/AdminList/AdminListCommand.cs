using MediatR;
using SC.SenseTower.Cinemas.Dto.Users;

namespace SC.SenseTower.Cinemas.Cqrs.AdminList
{
    public class AdminListCommand : IRequest<IEnumerable<UserInfoDto>>
    {
        /// <summary>
        /// Идентификатор кинотеатра.
        /// </summary>
        public Guid CinemaId { get; set; }
    }
}
