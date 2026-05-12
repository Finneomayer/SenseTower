using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mechanics.LoadSceneObjects.Models;

namespace Mechanics.Network.Scripts.SpaceObjectsService
{
    public interface ISpaceObjectService
    {
        public UniTask<Space> GetSpaceWithAllObjects(string spaceId);
    }
}
