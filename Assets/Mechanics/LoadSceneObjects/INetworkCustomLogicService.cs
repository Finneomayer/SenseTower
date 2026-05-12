using Mechanics.LoadSceneObjects.Models;

namespace Mechanics.LoadSceneObjects
{
    public interface INetworkCustomLogicService
    {
        public void Init(StaticObject staticObject, CustomBehaviourNetworkObject customBehaviourNetworkObject);
    }
}