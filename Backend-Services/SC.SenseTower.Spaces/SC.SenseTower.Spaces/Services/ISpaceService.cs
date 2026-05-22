using SC.SenseTower.Spaces.Spaces;

namespace SC.SenseTower.Spaces.Services;

public interface ISpaceService
{
    Task<TowerSpace[]> GetAllSpaces();
}