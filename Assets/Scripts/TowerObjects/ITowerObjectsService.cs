using Cysharp.Threading.Tasks;
using System;

namespace Assets.Scripts.TowerObjects
{
    public interface ITowerObjectsService
    {
        UniTask<TowerObjectDto[]> GetTowerObjects();
        UniTask<TowerObjectDto[]> GetUserObjects();
        //void AddUserObject(TowerObjectDto towerObject);
        //void RemoveUserObject(Guid guid);
    }
}