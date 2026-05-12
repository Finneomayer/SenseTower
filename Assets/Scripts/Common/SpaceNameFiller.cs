using System;
using System.Collections.Generic;
using Assets.Scripts.Hall;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Common
{
    public class SpaceNameFiller : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private List<SpaceData> _spaces = new();

        #endregion

        private IHallsService _hallService;

        [Inject]
        public void Init(IHallsService hallService)
        {
            _hallService = hallService;
        }

        private async void Start()
        {
            foreach (SpaceData space in _spaces)
            {
                string spaceName = await FindSpaceOnTheFloor(space.ChangeSceneAction.spaceType,
                    space.ChangeSceneAction.HallIndex);
                space.PlaceItemUI.SetName(spaceName);
            }
        }

        private async UniTask<string> FindSpaceOnTheFloor(SpaceType spaceType, int hallIndex)
        {
            string spaceName = string.Empty;

            Hall.Hall[] halls = await _hallService.GetHalls();
            if (halls.Length <= hallIndex)
            {
                Debug.LogError("halls.Length <= hallIndex. Cannot find space");
                return string.Empty;
            }

            if (spaceType == SpaceType.HallScene)
            {
                spaceName = halls[hallIndex].Space.SpaceName;
            }
            else
            {
                foreach (var localSpace in halls[hallIndex].Spaces)
                {
                    if (spaceType == localSpace.SpaceType)
                    {
                        spaceName = localSpace.SpaceName;
                        break;
                    }
                }
            }

            return spaceName;
        }

        #region InnerClass

        [Serializable]
        public class SpaceData
        {
            public PlaceItemUI PlaceItemUI;
            public ChangeSceneAction ChangeSceneAction;
        }

        #endregion
    }
}