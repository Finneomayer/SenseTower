namespace SC.SenseTower.Spaces.State.GetUsersInSpaces;

public record GetUsersInSpacesResponse(UserInSpaceInfoDto[] Users, int? GetCount, int TotalCount);