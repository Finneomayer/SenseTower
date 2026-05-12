using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.Space;
using Assets.Scripts.Trading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Mechanics.Doors
{
    public class DoorAccessController : MonoBehaviour
    {
        #region Inspector
        
        [SerializeField] private DoorPaidController _doorPaidController;
        private LocalSpace _localSpace;
        private ITradeService _tradeService;
        private IRegistrationInSpacesService _registrationInSpacesService;
        private ActiveDoor _activeDoor;

        #endregion

        [Inject]
        private void Construct(IRegistrationInSpacesService registrationInSpacesService)
        {
            _registrationInSpacesService = registrationInSpacesService;
        }

        public void SetDoorLocalSpace(LocalSpace localSpace, ActiveDoor activeDoor)
        {
            _activeDoor = activeDoor;
            _localSpace = localSpace;
        }

        public async UniTask<bool> AllowedDoorEnter()
        {
            var untc = new UniTaskCompletionSource<bool>();
            bool result = false;
            if (_localSpace != null && _localSpace.PublicAccessType == SpaceAccessType.Paid)
            { 
                result = await _doorPaidController.CheckDoorAccess(_localSpace);
                if (result)
                {
                    AccessResultDto accessData = await _registrationInSpacesService.CheckAccess(_activeDoor.SpaceId);
                    _activeDoor.SetAccessData(accessData);
                }
            }
            else
            {
                result = true;
            }

            untc.TrySetResult(result);
            
            return await untc.Task;
        }
    }
}