namespace SC.SenseTower.Tickets.Dto.Spaces
{
    public class SpaceConnectionInfoDto
    {
        /// <summary>
        /// Порт сервера.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// IP-адрес сервера.
        /// </summary>
        public string Ip { get; set; } = null!;
    }
}
