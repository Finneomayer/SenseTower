namespace SC.SenseTower.Images.Cqrs
{
    public class BaseRequest
    {
        /// <summary>
        /// Идентификатор изображения.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор текущего пользователя (заполняется сервером).
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Токен пользователя (заполняется сервером).
        /// </summary>
        public string? AccessToken { get; set; }
    }
}
