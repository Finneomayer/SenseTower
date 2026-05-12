using System.Collections.Generic;
using Assets.Mechanics.Tips.Model;
using Cysharp.Threading.Tasks;

namespace Assets.Mechanics.Tips
{
    public interface ITipsService
    {
        public UniTask<string> GetTipsFromID(string tipsId);
        public UniTask<List<TipsItemDto>> GetTipsFromIDs(List<string> tipsIds);
    }
}