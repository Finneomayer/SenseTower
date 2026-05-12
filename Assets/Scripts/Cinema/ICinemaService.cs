using Cysharp.Threading.Tasks;
using System;

namespace Assets.Scripts.Cinema
{
    public interface ICinemaService
    {
        public UniTask<Cinema> GetById(string id);
        public UniTask<Cinema> GetBySpaceId(string spaceId);
        public UniTask<Cinema[]> GetCinemas();
    }
}