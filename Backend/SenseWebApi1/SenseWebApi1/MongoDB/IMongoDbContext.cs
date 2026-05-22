using MongoDB.Driver;

namespace SenseWebApi1.MongoDB;

public interface IMongoDbContext
{
    IMongoDatabase GetMongoDatabase();
    Task EventIniz();
        
}