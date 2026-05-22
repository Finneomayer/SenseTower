namespace SC.SenseTower.MyPlaces.Dto.Places
{
    public class SpaceConnectionDto
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
