using UnityEngine;
using UnityEngine.UI;

namespace Assets.Mechanics.ScrollRectFix
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectBlocker : MonoBehaviour
    {
        [SerializeField] private int _maxItemCountScrollBlocked = 0;
        [SerializeField] private RectTransform _itemsContainer;

        private ScrollRect _scrollRect;

        void Start()
        {
            _scrollRect = GetComponent<ScrollRect>();
        }

    
        void Update()
        {
            if (_scrollRect != null && _maxItemCountScrollBlocked > 0)
            {
                if (_itemsContainer != null)
                {
                    if (_itemsContainer.childCount > _maxItemCountScrollBlocked) _scrollRect.vertical = true;
                    else _scrollRect.vertical = false;
                }
            }
        }
    }
}
