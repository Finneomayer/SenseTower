namespace SC.SenseTower.Spaces.Services;

public interface IUserLocationService
{
    void RegisterUsersInSpace(Guid placeId, Guid[] userIds);
    bool IsUserInSpace(Guid placeId, Guid userId);
    List<(Guid userId, Guid spaceId)> GetUsersInSpaces();
}