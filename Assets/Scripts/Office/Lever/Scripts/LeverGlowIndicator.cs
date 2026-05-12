using Unity.Netcode;
using UnityEngine;

public class Indicator : NetworkBehaviour
{
    public float IndicatorActive { get; set; }
}

public class LeverGlowIndicator : Indicator
{
    public Color _glowColor;
    protected float lastIsActive = -1;

    private Material[] modelMaterials;
    private Color updatedColor;

    void Start () 
    {

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        modelMaterials = new Material[renderers.Length];
        for (int i = 0; i < modelMaterials.Length; i++)
        {
            this.modelMaterials[i] = renderers[i].material;
        }

        foreach (Material material in this.modelMaterials)
        {
            material.EnableKeyword("_EMISSION");
        }
        updatedColor = new Color(0, 0, 0);
    }

    void Update ()
    {
        if (lastIsActive != IndicatorActive)
        {
            lastIsActive = IndicatorActive;
            SetEmmission(IndicatorActive);
        }
    }

    private void SetEmmission(float indicatorActive)
    {
        updatedColor.r = _glowColor.r * indicatorActive * _glowColor.a;
        updatedColor.g = _glowColor.g * indicatorActive * _glowColor.a;
        updatedColor.b = _glowColor.b * indicatorActive * _glowColor.a;

        foreach (Material material in modelMaterials)
        {
            material.SetColor("_EmissionColor", updatedColor);
        }
    }
}
