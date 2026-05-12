using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Localization;
using Assets.Scripts.Audio;
using Assets.Scripts.Cinema;
using Assets.UI.Pad;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UI_assets_legacy.Pad
{
    public class ZoneView : MonoBehaviour
    {
        [SerializeField] private VisitorAdminViewElement _visitorAdminViewElement; //with buttons
        [SerializeField] private VisitorNonAdminViewElement _visitorNonAdminViewElement; //no buttons
        [SerializeField] private Transform _panelZoneAdmin;
        [SerializeField] private Transform _panelZoneVisitors;
        [SerializeField] private ZoneController _zoneController;
        [SerializeField] private Canvas _zoneCanvas;
        [SerializeField] private Toggle muteToogle;
        [SerializeField] private Toggle _lockToggle;

        [Header("Localization")]
        [SerializeField] private LocalizationVariant _labelAdmin;
        [SerializeField] private LocalizationVariant _labelMe;

        [SerializeField] private AgoraAudioService _audioService;
        private void OnEnable()
        {
#if !UNITY_SERVER
            _zoneController.AdminChanged += ZoneControllerAdminChanged;
            _zoneController.ParticipantChanged += ZoneControllerParticipantChanged;
            _zoneController.MuteChanged += ZoneControllerOnMuteChanged;
            _zoneController.LockChanged += ZoneControllerOnLockChanged;
            muteToogle.onValueChanged.AddListener(OnMuteToggled);
            _lockToggle.onValueChanged.AddListener(OnLockToggled);
            
            _audioService = FindObjectOfType<AgoraAudioService>();
#endif
        }

        private void OnDisable()
        {
#if !UNITY_SERVER
            _zoneController.AdminChanged -= ZoneControllerAdminChanged;
            _zoneController.ParticipantChanged -= ZoneControllerParticipantChanged;
            _zoneController.MuteChanged -= ZoneControllerOnMuteChanged;
            _zoneController.LockChanged -= ZoneControllerOnLockChanged;
            muteToogle.onValueChanged.RemoveListener(OnMuteToggled);
            _lockToggle.onValueChanged.RemoveListener(OnLockToggled);
#endif           
        }

        private void Start()
        {
#if !UNITY_SERVER            
            _zoneCanvas.renderMode = RenderMode.WorldSpace;
            _zoneCanvas.worldCamera = Camera.main;
#endif
        }

        private void ZoneControllerOnMuteChanged(bool value)
        {
            muteToogle.isOn = value;
        }

        private void ZoneControllerOnLockChanged(bool value)
        {
            _lockToggle.isOn = value;
        }

#if !UNITY_SERVER
        private void ZoneControllerParticipantChanged(Dictionary<ulong,string> users, bool clientIsAdmin)
        {
            if(ReferenceEquals(_panelZoneVisitors, null)) return;
            
            DestroyChildren(_panelZoneVisitors.gameObject);
            if (clientIsAdmin) CreateParticipantItemForAdmin(users, _panelZoneVisitors);
            else CreateParticipantItem(users, _panelZoneVisitors);
        }

        private void ZoneControllerAdminChanged(ulong id,string adminName, bool clientIsAdmin)
        {
            DestroyChildren(_panelZoneAdmin.gameObject);

            if (id != 0) CreateParticipantItem(new Dictionary<ulong,string>(){[id]=adminName}, _panelZoneAdmin);

        }

        private void DestroyChildren(GameObject parent)
        {
            var children1 = parent.GetComponentsInChildren<VisitorAdminViewElement>();
            foreach (var child in children1)
            {
                Destroy(child.gameObject);
            }
            var children2 = parent.GetComponentsInChildren<VisitorNonAdminViewElement>();
            foreach (var child in children2)
            {
                Destroy(child.gameObject);
            }
        }

        private void CreateParticipantItem(Dictionary<ulong,string> users, Transform parent)
        {
            foreach (var user in users)
            {
                var element = Instantiate(_visitorNonAdminViewElement, parent);
                ulong id = user.Key;
                element.SetId(id);
                string resultLabel = String.Empty;

                if (id == _zoneController.ZonesModel.OwnerId)
                {
                    if (resultLabel != "")
                    {
                        resultLabel += ", ";
                    }
                    resultLabel += $"{_labelMe.Localize()},{_labelAdmin.Localize()}";
                }
                element.SetName(user.Value);
                element.SetText(user.Value);
                element.SetLabel(resultLabel);
            }
        }

        private void CreateParticipantItemForAdmin(Dictionary<ulong,string> users, Transform parent)
        {
            DestroyChildren(parent.gameObject);
            foreach (var id in users)
            {
                if(id.Key == _zoneController.ZonesModel.OwnerId) continue;
                var element = Instantiate(_visitorAdminViewElement, parent);
                element.SetId(id.Key);
                element.SetName(id.Value);
                element.SetText(id.Value);
                
                bool existUserinMuteList = false;
                if(_audioService != null)
                    existUserinMuteList = _audioService.MutedUsersID.ContainsKey(id.Key);
                element.ToogleMuteState(existUserinMuteList);
                
                element.AdminButtonClicked += Element_AdminButtonClicked;
                element.MuteButtonClicked += Element_MuteButtonClicked;
                element.KickButtonClicked += Element_KickButtonClicked;
            }
        }

        private void Element_KickButtonClicked(ulong id, Guid guid)
        {
            _zoneController.KickParticipantServerRPC(id);
        }

        private void Element_MuteButtonClicked(ulong id, bool isMuted)
        {
            if (_audioService == null)
                return;
            
            if (isMuted)
            {
                _audioService.MuteUser(id);
            }
            else
            {
                _audioService.UnmuteUser(id);
            }
        }

        private void Element_AdminButtonClicked(ulong id)
        {
            _zoneController.ChangeAdmin(id);
        }

        public void ChangeAudioChannelOfParticipants(bool isMute)
        {
            _zoneController.ChangeAudioChannelServerRpc(isMute);
        }

        private void OnLockToggled(bool newValue)
        {
            _zoneController.ChangeLockedServerRpc(newValue);
            InteractableAfterTime();
        }

        private void OnMuteToggled(bool newValue)
        {
            ChangeAudioChannelOfParticipants(newValue);
            InteractableAfterTime();
        }

        [ContextMenu("Toggle mute")]
        private void ToggleMute()
        {
            muteToogle.isOn = !muteToogle.isOn;
            OnMuteToggled(muteToogle.isOn);
        }

        [ContextMenu("Toggle lock")]
        private void ToggleLock()
        {
            _lockToggle.isOn = !_lockToggle.isOn;
            OnLockToggled(_lockToggle.isOn);
        }
#endif
        public void InteractableAfterTime()
        {
            if(gameObject.activeSelf)
                StartCoroutine(InteractableAfterTimeCoroutine());
        }

        private IEnumerator InteractableAfterTimeCoroutine()
        {
            muteToogle.interactable = false;
            _lockToggle.interactable = false;
            yield return new WaitForSeconds(1.4f);
            muteToogle.interactable = true;
            _lockToggle.interactable = true;
        }
    }
}
