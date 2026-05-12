using Assets.Scripts.Space;

public class SpaceDoorData
{
    public bool IsPrivate { get; } = false;
    public SpaceType SpaceType { get; } = SpaceType.Null;
    public string SpaceId { get; } = string.Empty;

    public SpaceDoorData(bool isPrivate, SpaceType spaceType, string spaceId)
    {
        IsPrivate = isPrivate;
        SpaceType = spaceType;
        SpaceId = spaceId;
    }
}
