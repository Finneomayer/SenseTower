using System;
using System.Collections.Generic;

namespace Assets.Mechanics.Tips
{
    public class TipsSceneRepository : ITipsSceneRepository
    {
        public event Action OnTipsChanged;

        private string _currentTipsId;
        private string _tipsText;
        private List<TipsViewer> _viewers = new();

        public void RegisterViewer(TipsViewer tipsViewer)
        {
            if (!_viewers.Contains(tipsViewer))
            {
                _viewers.Add(tipsViewer);
            }
        }

        public List<TipsViewer> GetRegisteredViewers()
        {
            return _viewers;
        }

        public void SetTipsText(string tipsText, string senderId)
        {
            _currentTipsId = senderId;
            _tipsText = tipsText;
            OnTipsChanged?.Invoke();
        }

        public void ClearTipsText(string senderId)
        {
            if (string.IsNullOrEmpty(_currentTipsId))
            {
                SetTipsText(string.Empty, string.Empty);
            }
            
            if (!_currentTipsId.Equals(senderId))
                return;

            SetTipsText(string.Empty, string.Empty);
        }

        public string GetTipsText()
        {
            return _tipsText;
        }
    }
}