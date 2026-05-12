using Assets.Scripts.TowerObjectsRevision.Models;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.TowerObjectsRevision.Interfaces
{
    public interface ITowerObjectsRevision
    {
        public UniTask<string> SaveTowerObject(SaveTowerObjectRequestDTO towerObjectRequestDto);
        public UniTask<TowerObjectRevisionDTO[]> GetAllTowerObjects(string id);
        
    }
}