using System;
using System.Collections;
using Assets.Mechanics.Doors;
using Assets.Scripts.Hall;
using Assets.Scripts.Player;
using Assets.Scripts.Player.WindowsMovement;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Management;
using Zenject;
using Random = UnityEngine.Random;

namespace Assets.Mechanics.Lift
{
    public static class PlayerLiftPosition
    {
        public static int PlayerInHallIndex;
        public static int PlayerOnFloorIndex;
        public static bool PutPlayerToTheLift;
    }

    public class LiftController : MonoBehaviour
    {
        [SerializeField] private int _liftFloorIndex;
        [SerializeField] private LiftButton[] _buttons;
        [SerializeField] private ActiveDoorController _doors;
        [SerializeField] private CompositionRootNetworkScene _compositionRootEnterScene;

        public event Action ButtonSuccessfulClick;
        public event Action ButtonUnSuccessfulClick;
        public event Action LiftFinished;

        private CompositionRootNetworkScene _compositionRoot;
        private PlayerLogic _player;
        private EditorMovementSystem _playerMover;

        private ISpaceService _spaceService;
        private IHallsService _hallService;

        [Inject]
        public void Init(ISpaceService spaceService, IHallsService hallService)
        {
            _spaceService = spaceService;
            _hallService = hallService;
        }

        private void Awake()
        {
            _compositionRoot = FindObjectOfType<CompositionRootNetworkScene>();
        }

        private void Start()
        {
            GetPlayer();
        }

        private void OnEnable()
        {
            foreach (var button in _buttons)
            {
                button.OnLiftButtonPressed += Button_OnLiftButtonPressed;
            }
        }

        private void OnDisable()
        {
            foreach (var button in _buttons)
            {
                button.OnLiftButtonPressed -= Button_OnLiftButtonPressed;
            }
        }

        private async void GetPlayer()
        {
            _player = await _compositionRoot.GetLocalPlayerAsync();
        }

        private void Button_OnLiftButtonPressed(LiftButtonTarget target)
        {
            if (target.HallIndex == _doors.CurrentHallId)
            {
                if (target.FloorIndex == _liftFloorIndex)
                {
                    ButtonUnSuccessfulClick?.Invoke();
                    return;
                }

                ButtonSuccessfulClick?.Invoke();
                PlayerLiftPosition.PlayerOnFloorIndex = target.FloorIndex;
                StartCoroutine(TeleportLocal(target.ExitPoints));
            }
            else
            {
                ButtonSuccessfulClick?.Invoke();
                TeleportToOtherScene(target);
            }
        }

        private IEnumerator TeleportLocal(Transform[] points)
        {
            yield return new WaitForSeconds(1f);

            int i = Random.Range(0, points.Length);

            if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
            {
                _player.transform.position = points[i].position;
                _player.transform.rotation = points[i].rotation;
            }
            else
            {
                if (_playerMover == null)
                    _playerMover = _player.GetComponent<EditorMovementSystem>();
                _playerMover.SetPosition(points[i].position, points[i].rotation);
            }
            LiftFinished?.Invoke();
            yield return null;
        }

        private void TeleportToOtherScene(LiftButtonTarget target)
        {
            TryLoadScene(target).Forget();
        }

        private async UniTask TryLoadScene(LiftButtonTarget target)
        {
            string targetSpaceId;
            (Guid hallSpaceId, string spaceName) = await FindSpaceByHallIndex(SpaceType.HallScene, target.HallIndex);
            if (hallSpaceId != Guid.Empty)
            {
                targetSpaceId = hallSpaceId.ToString();
                //PlayerPrefs.SetInt("PlayerInHallIndex", target.HallIndex); //saving Hall index at start
                //PlayerPrefs.SetInt("PlayerOnFloor", target.FloorIndex); //saving first floor of Hall at start

                PlayerLiftPosition.PlayerInHallIndex = target.HallIndex; //saving choosen Hall index
                PlayerLiftPosition.PlayerOnFloorIndex = target.FloorIndex; //saving choosen floor index
                PlayerLiftPosition.PutPlayerToTheLift = true; //tell that need to put player near lift on next spawn

                _compositionRootEnterScene.SceneChanger.ChangeSpace(SpaceType.HallScene, targetSpaceId, spaceName);
            }
        }


        private async UniTask<(Guid, string)> FindSpaceByHallIndex(SpaceType spaceType, int hallIndex)
        {
            Guid spaceId = Guid.Empty;
            string spaceName = "";

            Hall[] halls = await _hallService.GetHalls();
            if (halls.Length <= hallIndex)
            {
                Debug.LogError("halls.Length <= hallIndex. Cannot find space");
                return (spaceId, spaceName);
            }

            if (spaceType == SpaceType.HallScene)
            {
                spaceId = halls[hallIndex].Space.Id;
                spaceName = halls[hallIndex].Space.SpaceName;
            }
            else
            {
                foreach (var localSpace in halls[hallIndex].Spaces)
                {
                    if (spaceType == localSpace.SpaceType)
                    {
                        spaceId = localSpace.Id;
                        spaceName = localSpace.SpaceName;
                        break;
                    }
                }
            }
            return (spaceId, spaceName);
        }
    }
}