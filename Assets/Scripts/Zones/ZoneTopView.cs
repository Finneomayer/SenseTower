using Unity.Netcode;
using UnityEngine;

public sealed class ZoneTopView : MonoBehaviour
{
    [SerializeField]
    private ZoneController ZoneController;
    [SerializeField]
    private Renderer TopRenderer;
    [SerializeField]
    private string ShaderColorPropertyName = "Color";
    [SerializeField]
    private Color MuteColor;
    [SerializeField]
    private Color LockColor;

    private void OnEnable()
    {
        ZoneController.MuteChanged += OnZoneMuteChanged;
        ZoneController.LockChanged += OnZoneLockChanged;
        
    }

    private void OnDisable()
    {
        ZoneController.MuteChanged -= OnZoneMuteChanged;
        ZoneController.LockChanged -= OnZoneLockChanged;
    }

    private void OnZoneMuteChanged(bool muted)
    {
        RefreshRenderer();
    }

    private void OnZoneLockChanged(bool locked)
    {
        RefreshRenderer();
    }

    private void RefreshRenderer()
    {
        if (!ZoneController.IsMuted && !ZoneController.IsLocked)
        {
            TopRenderer.gameObject.SetActive(false);
            return;
        }
        Color newColor = ZoneController.IsLocked ? LockColor : MuteColor;
        TopRenderer.material.SetColor(ShaderColorPropertyName, newColor);

        TopRenderer.gameObject.SetActive(true);
    }
}
