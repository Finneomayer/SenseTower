namespace SC.SenseTower.Spaces.State.GetUsersInSpaces;

public record UserInSpaceInfoDto(Guid SpaceId, Guid UserId, string UserName, string SpaceName);