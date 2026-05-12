using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Mechanics.Mafia.UI
{
    public enum VoteCellType
    {
        Kill,
        Love,
        Check,
        Heal
    }

    [Serializable]
    public class VoteCellItem
    {
        public VoteCellType CellType;
        public Image Image;
    }

    public class MafiaVoteCellView : MonoBehaviour
    {
        [SerializeField] private VoteCellItem[] _items;

        private Image _currentItem;
        private Color _baseColor = Color.white;
        private Color _localUserVotedColor = Color.white;
        private VoteCellType _cellType;

        public void Init(VoteCellType type, Color localUserVotedColor)
        {
            _cellType = type;
            SetImageByType(_cellType);
            _localUserVotedColor = localUserVotedColor;
        }

        public void SetVoteActive(bool active, bool isLocalUserVoted = false)
        {
            if (_currentItem !=  null && _cellType == VoteCellType.Kill) 
                _currentItem.color = isLocalUserVoted ? _localUserVotedColor : _baseColor;
            gameObject.SetActive(active);
        }

        private void SetImageByType(VoteCellType type)
        {
            foreach (var item in _items)
            {
                item.Image.gameObject.SetActive(false);
                if (item.CellType == type)
                {
                    item.Image.gameObject.SetActive(true);
                    _currentItem = item.Image;
                }
            }
        }
    }
}