using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Spaces.Spaces;

public sealed class TowerSpace
{
    public Guid Id { get; set; }
    public SpaceConnectionInfo? SpaceConnectionInfo { get; set; }
    public SpaceType SpaceType { get; set; }
    public string? RemoteSceneName { get; set; }
    public string SceneName { get; set; }
    public string SpaceName { get; set; }
};