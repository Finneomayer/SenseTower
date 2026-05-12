using System;
using System.Collections.Generic;

namespace Assets.Mechanics.Tips
{
    public interface ITipsSceneRepository
    {
        public event Action OnTipsChanged;
        public void SetTipsText(string tipsText,string senderId);
        public void ClearTipsText(string senderId);
        public string GetTipsText();
        
        public void RegisterViewer(TipsViewer tipsViewer);
        public List<TipsViewer> GetRegisteredViewers();
    }
}