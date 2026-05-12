using Cysharp.Threading.Tasks;

namespace Assets.Mechanics.Tips
{
    public interface ITipsSceneContext
    {
        public void RegisterTipsId(string tipsId);
        public void GetAllTips();
        public UniTask<string> GetTipsFromId(string tipsId);
        public void ShowTips();
        public void CleanUp();
    }
}