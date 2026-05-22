


namespace SenseWebApi1.Context;

public interface IEventContext
{
        

    Task UpdateEventForImage(Guid imageId);

    Task DeleteEventForSpace(Guid spaceId);
}