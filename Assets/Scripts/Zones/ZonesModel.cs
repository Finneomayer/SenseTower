using Assets.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Mechanics.Mafia.Table;
using Assets.Scripts.Client;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Zenject;
using Assets.Scripts.Player.WindowsMovement;
using Sense.Interectable.Teleportation;
using Assets.Mechanics.Doors;

namespace Assets.Scripts.Zones
{
    public class ZonesModel : MonoBehaviour
    {
        [field: Header("Autofill data when PlayerOwner inits")]

        [field: SerializeField]
        public PlayerLogic PlayerOwner { get; private set; }

        [field: SerializeField]
        public ulong OwnerId { get; private set; } //comes from NetworkPlayer script only if it's OWNER

        [field: SerializeField]
        public ZoneController ZoneController { get; private set; }

        [field: SerializeField]
        public OnPlayerUI OnPlayerUi { get; private set; }

        [field: SerializeField]
        public Place Place { get; private set; }
        
        [field: SerializeField]
        public AgoraVoice Agora { get; private set; }

        public bool IsTeleportingAllowed { get; private set; } = true;

        private TeleportationProvider _teleportationProvider;
        private EditorMovementSystem _winMovementSystem;
        private IClientData _clientData;
        private Vector3 _positionBeforeSitting;
        private Quaternion _rotationBeforeSitting;

        [Space] 
        [SerializeField] private List<ZoneExpander> _expandedZones;
        [SerializeField] private List<TableExpander> _expandedTables;

        [Inject]
        private void Construct(IClientData clientData)
        {
            _clientData = clientData;
        }

        private void OnEnable()
        {
            if (_winMovementSystem != null)
            {
                _winMovementSystem.Jumped -= OnJump;
                _winMovementSystem.Jumped += OnJump;
            }
        }

        private void OnDisable()
        {
            if (_winMovementSystem != null)
            {
                _winMovementSystem.Jumped -= OnJump;
            }
        }

        public void Init(ulong id, PlayerLogic playerOwner, OnPlayerUI playerUi)
        {
            OwnerId = id;
            PlayerOwner = playerOwner;
            OnPlayerUi = playerUi;
            _teleportationProvider = playerOwner.GetComponent<TeleportationProvider>();

            if (Application.platform != RuntimePlatform.Android)
            {
                _winMovementSystem = playerOwner.GetComponent<EditorMovementSystem>();
                _winMovementSystem.Jumped -= OnJump;
                _winMovementSystem.Jumped += OnJump;
            }

            //if (_expandedZones != null)
            //{
            //    foreach (var zone in _expandedZones)
            //    {
            //        zone.Init(playerUi);
            //    }
            //}
            //if (_expandedTables != null)
            //{
            //    foreach (var table in _expandedTables)
            //    {
            //        table.Init(playerUi);
            //    }
            //}
        }

        public string GetPlayerName()
        {
            return _clientData.UserName;
        }
        public void SetZone(ZoneController zoneController)
        {
            ZoneController = zoneController;
        }

        public void SavePlayerPositionAndRotation()
        {
            _positionBeforeSitting = PlayerOwner.transform.position;
            _rotationBeforeSitting = PlayerOwner.transform.rotation;
        }

        public void RestorePlayerPositionAndRotation()
        {
            if (_winMovementSystem != null)
            {
                _winMovementSystem.SetEnabledMovement(true);
                _winMovementSystem.SetPosition(_positionBeforeSitting, _rotationBeforeSitting);
            }
        }

        public void SetPlace(Place place)
        {
            Place = place;

            if (_winMovementSystem != null && place != null)
            {
                _winMovementSystem.SetEnabledMovement(false);
                _winMovementSystem.SetPosition(place.TeleportAnchor.TeleportAnchorTransform.position,
                    place.TeleportAnchor.TeleportAnchorTransform.rotation);
            }
        }

        public TeleportationProvider GetTeleportationProvider()
        {
            return _teleportationProvider;
        }

        public void SetTeleportingAllowed(bool allowed, bool allTeleports)
        {
            IsTeleportingAllowed = allowed;

            if (allTeleports)
            {
                var teleports = FindObjectsOfType<Place>();
                foreach (var place in teleports)
                {
                    place.SwitchPlaceCollider(allowed);
                }
                var teleportAreas = FindObjectsOfType<TeleportationArea>();
                foreach (var teleport in teleportAreas)
                {
                    teleport.enabled = allowed;
                }
                var doors = FindObjectsOfType<ActiveDoor>();
                foreach (var door in doors)
                {
                    door.enabled = allowed;
                }
            }
        }

        public void LeavePlace()
        {
            if (IsTeleportingAllowed && Place != null)
            {
                Place.LeavePlace();
            }
        }

        private void OnJump()
        {
            LeavePlace();
        }
    }
}
