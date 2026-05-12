using Assets.Scripts.Zones;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleTeleportAreasZoneController : MonoBehaviour
{
    [SerializeField]
    private ZonesModel ZonesModel;
    [SerializeField]
    private Transform TeleportAreasContent;

    private BaseTeleportationInteractable[] _teleportAreas;
    private PointerEventsHandler[] _pointerEventsHandlers;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        RegisterListeners();
    }

    private void OnDisable()
    {
        UnregisterListeners();
    }

    public void SetTeleportArea(Transform teleportAreasContent) 
    {
        TeleportAreasContent = teleportAreasContent;
    }

    public void Init() 
    {
        if (ZonesModel == null || TeleportAreasContent == null) return;

        UnregisterListeners();
        
        _teleportAreas = TeleportAreasContent.GetComponentsInChildren<BaseTeleportationInteractable>();

        if (Application.platform != RuntimePlatform.Android)
        {
            _pointerEventsHandlers = new PointerEventsHandler[_teleportAreas.Length];

            for (int i = 0; i < _teleportAreas.Length; i++)
            {
                var area = _teleportAreas[i];
                if (!area.gameObject.TryGetComponent(out PointerEventsHandler pointerEventsHandler))
                {
                    pointerEventsHandler = area.gameObject.AddComponent<PointerEventsHandler>();
                }
                _pointerEventsHandlers[i] = pointerEventsHandler;
            }
        }

        RegisterListeners();
    }

    public void RegisterListeners()
    {
        UnregisterListeners();
        if (_teleportAreas == null)
        {
            return;
        }

        foreach (var area in _teleportAreas)
        {
            area.teleporting.AddListener(OnTeleporting);
        }

        if (_pointerEventsHandlers != null)
        {
            foreach (var handler in _pointerEventsHandlers)
            {
                handler.Clicked += AreaClicked;
            }
        }
    }

    public void UnregisterListeners()
    {
        if (_teleportAreas == null)
        {
            return;
        }

        foreach (var area in _teleportAreas)
        {
            area.teleporting.RemoveListener(OnTeleporting);
        }

        if (_pointerEventsHandlers != null)
        {
            foreach (var handler in _pointerEventsHandlers)
            {
                handler.Clicked -= AreaClicked;
            }
        }
    }

    private void OnTeleporting(TeleportingEventArgs teleportingEventArgs)
    {
        LeavePlace();
    }

    private void AreaClicked()
    {
        LeavePlace();
    }

    private void LeavePlace()
    {
        if (ZonesModel != null)
        {
            ZonesModel.LeavePlace();
        }
    }
}
