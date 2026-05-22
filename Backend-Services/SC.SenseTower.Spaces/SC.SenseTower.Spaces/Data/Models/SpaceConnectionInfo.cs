namespace SC.SenseTower.Spaces.Data.Models
{
    public class SpaceConnectionInfo
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