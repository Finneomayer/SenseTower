using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mechanics.LoadSceneObjects.Models;

namespace Mechanics.Network.Scripts.StaticObjectsService
{
    public interface IStaticObjectsService
    {
        public UniTask<bool> SaveSceneStaticObjects(List<StaticObject> objects,string spaceId);
        //public UniTask<List<StaticObject>> GetClientSceneStaticObjects(string spaceId);
        //public UniTask<List<StaticObject>> GetServerSceneStaticObjects(string spaceId);
        public UniTask<List<StaticObject>> GetAllSceneStaticObjects(string spaceId);
    }
}