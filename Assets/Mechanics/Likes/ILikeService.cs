using System;
using Assets.Scripts.Client;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Space
{
    public interface ILikeService
    {
        public UniTask<bool> Like(Guid spaceId, bool? like, IClientData clientData);
    }
}