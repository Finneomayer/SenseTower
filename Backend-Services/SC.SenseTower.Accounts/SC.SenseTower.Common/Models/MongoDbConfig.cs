namespace SC.SenseTower.Common.Models
{
    public class MongoDbConfig
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public bool AllowInsecureTls { get; set; } = true;
    }
}
