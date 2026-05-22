namespace SC.SenseTower.Auth.Settings
{
    public class MongoDbConfig
    {
     //   public string ConnectionString => $"mongodb://{Host}:{Port}";

        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }
    }
}
