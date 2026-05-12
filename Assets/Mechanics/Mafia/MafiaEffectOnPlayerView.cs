using System.Collections.Generic;
using Assets.Mechanics.Mafia.UI;
using UnityEngine;

namespace Assets.Mechanics.Mafia
{
    public class MafiaEffectOnPlayerView : MonoBehaviour
    {
        private const int EffectsMaxCount = 4;

        [SerializeField] private Transform _effectCellContainer;
        [SerializeField] private MafiaVoteCellView _effectCellPrefab;

        private MafiaVoteCellView[] _cells;

        public void Init(MafiaGameStage gameStage, Dictionary<Effect, int> effects)
        {
            if (gameStage is MafiaGameStage.Voting 
                or MafiaGameStage.CommonDiscussion 
                or MafiaGameStage.DeadManLastWords 
                or MafiaGameStage.StartOfDay 
                or MafiaGameStage.VotingResultsDiscussion)
            {
                Clear();

                CreateCells();

                int i = 0;

                foreach (var effect in effects)
                {
                    if (i >= EffectsMaxCount)
                    {
                        Debug.LogError("Dictionary<Effect, int> > EffectsMaxCount");
                        break;
                    }

                    if (effect.Value > 0)
                    {
                        if (effect.Key == Effect.VotingProtection)
                        {
                            _cells[i].Init(VoteCellType.Love, Color.white);
                            _cells[i].SetVoteActive(true);
                            i++;
                        }
                    }
                }
            }
        }

        private void CreateCells()
        {
            if (_cells != null)
            {
                return;
            }

            _cells = new MafiaVoteCellView[EffectsMaxCount];

            for (int i = 0; i < _cells.Length; i++)
            {
                _cells[i] = Instantiate(_effectCellPrefab, _effectCellContainer.position,
                    _effectCellContainer.rotation, _effectCellContainer);
                _cells[i].SetVoteActive(false);
            }
        }

        private void Clear()
        {
            if (_cells == null)
            {
                return;
            }

            foreach (var item in _cells)
            {
                item.SetVoteActive(false);
            }
        }
    }
}
