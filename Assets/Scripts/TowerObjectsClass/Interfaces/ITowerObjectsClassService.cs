using Assets.Scripts.TowerObjectsClass.Models;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts
{
    public interface ITowerObjectsClassService
    {
        public UniTask<TowerObjectClass[]> GetAllTowerObjectClass();
    }
}