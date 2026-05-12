using Cysharp.Threading.Tasks;
using System;

namespace Assets.Scripts.Hall
{
    public interface IHallsService
    {
        public UniTask<Hall[]> GetHalls();
        public UniTask<Hall> FindHallBySpace(Guid spaceId);
    }
}