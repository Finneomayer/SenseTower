using System;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Space
{
    public interface ISpaceManager
    {
        void ChangeSpace(SpaceType type, string key = null, bool reload = false);
        LocalSpace CurrentTransitionTarget { get; }
        public UniTask<string> FindHallNameOfCurrentSpace();
    }
}