using UnityEngine;

namespace Assets.Mechanics.Mafia.UI
{

    public class MafiaVotesCountView : MonoBehaviour
    {
        private const int MaxVoteCount = 20;

        [SerializeField]
        private Transform VoteCellContainer;
        [SerializeField]
        private MafiaVoteCellView VoteCellPrefab;

        private MafiaVoteCellView[] _voteCells;
        private int _lastActiveCellIndex = -1;

        private Color _selectedColor;

        public void Init(GameState gameState, int activeCellsCount, Color selectedColor)
        {
            _selectedColor = selectedColor;
            Clear();

            if (activeCellsCount <= 0)
            {
                return;
            }

            CreateCells();

            if (activeCellsCount > _voteCells.Length)
            {
                Debug.LogError("MafiaVotesCountView. activeCellsCount > _voteCells.Length");
            }

            for (int i = 0; i < activeCellsCount; i++)
            {
                if (i >= _voteCells.Length)
                {
                    break;
                }

                _voteCells[i].Init(GetVoteCellTypeFromGameState(gameState), selectedColor);
                _voteCells[i].SetVoteActive(true);
                _lastActiveCellIndex = i;
            }

            //if (isLocalUserVoted && activeCellsCount <= _voteCells.Length)
            //{
            //    _voteCells[activeCellsCount - 1].SetVoteActive(true, true);
            //}
        }

        public void SetSelected(bool selected)
        {
            if (_voteCells == null)
            {
                return;
            }
            if (_lastActiveCellIndex < 0 || _lastActiveCellIndex >= _voteCells.Length)
            {
                return;
            }
            _voteCells[_lastActiveCellIndex].SetVoteActive(true, selected);
        }

        private void Clear()
        {
            _lastActiveCellIndex = -1;
            if (_voteCells == null)
            {
                return;
            }

            foreach (var item in _voteCells)
            {
                item.SetVoteActive(false);
            }
        }

        private void CreateCells()
        {
            if (_voteCells != null)
            {
                return;
            }

            _voteCells = new MafiaVoteCellView[MaxVoteCount];

            for (int i = 0; i < _voteCells.Length; i++)
            {
                _voteCells[i] = Instantiate(VoteCellPrefab, VoteCellContainer.position,
                    VoteCellContainer.rotation, VoteCellContainer);
                _voteCells[i].SetVoteActive(false);
            }
        }

        private VoteCellType GetVoteCellTypeFromGameState(GameState gameState)
        {
            VoteCellType result = VoteCellType.Kill;

            switch (gameState.GameStage)
            {
                case MafiaGameStage.CommissionerTurn: result = VoteCellType.Check; break;
                case MafiaGameStage.DoctorTurn: result = VoteCellType.Heal; break;
                case MafiaGameStage.LoverTurn: result = VoteCellType.Love; break;
            }

            return result;
        }
    }
}