namespace SC.SenseTower.Galleries.Dto.Spaces
{
    public class ConnectionInfoDto
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
