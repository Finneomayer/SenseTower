using SC.SenseTower.Cinemas.Dto.Spaces;

namespace SC.SenseTower.Cinemas.Dto.Cinemas
{
    public class CinemaDto
    {
        /// <summary>
        /// Идентификатор кинотеатра.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название кинотеатра.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Пространство, к которому привязан кинотеатр.
        /// </summary>
        public SpaceDto Space { get; set; } = new();

        /// <summary>
        /// Список администраторов кинотеатра.
        /// </summary>
        public Users.UserInfoDto[] Administrators { get; set; } = Array.Empty<Users.UserInfoDto>();
    }
}
