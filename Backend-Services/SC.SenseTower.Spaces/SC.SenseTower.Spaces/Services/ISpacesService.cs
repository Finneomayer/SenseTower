using SC.SenseTower.Common.Enums;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Spaces.Data.Models;

namespace SC.SenseTower.Spaces.Services;

public interface ISpacesService
{
    Task<Space?> Get(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Space>?> GetByOwnerId(Guid ownerId, CancellationToken cancellationToken);
    Task<Guid> Create(Space space, CancellationToken cancellationToken);
    Task Update(Space space, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Space>> GetAll(CancellationToken cancellationToken);
    Task<LookupItemDto<Guid>[]> Lookup(SpaceType? spaceType, CancellationToken cancellationToken);
    Task ReplaceImages(Guid spaceId, Picture[] pictures, CancellationToken cancellationToken);
    Task<bool> DeleteImage(Guid spaceId, Guid imageId, CancellationToken cancellationToken);
    Task ClearOwner(Guid userId, CancellationToken cancellationToken);
}