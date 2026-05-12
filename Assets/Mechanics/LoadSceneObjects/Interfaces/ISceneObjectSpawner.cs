using System.Collections.Generic;
using Mechanics.LoadSceneObjects.Models;
using Mechanics.Network.Scripts.SpaceObjectsService;
using Mechanics.VideoService.Models;

namespace Mechanics.LoadSceneObjects.Interfaces
{
    public interface ISceneObjectSpawner
    {
        public void SpawnSceneObjects(List<StaticObject> objects, List<PictureSpaceObject> pictures, List<VideoSpaceObject> videos);
    }
}
