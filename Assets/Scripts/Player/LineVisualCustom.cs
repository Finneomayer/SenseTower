using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LineVisualCustom : XRInteractorLineVisual
{
    [SerializeField]
    Gradient m_BlankColorGradient = new Gradient
    {
        colorKeys = new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 0f) },
        alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 0f) },
    };
    /// <summary>
    /// Controls the color of the line as a gradient from start to end to indicate an invalid state.
    /// </summary>
    public Gradient blankColorGradient
    {
        get => m_BlankColorGradient;
        set => m_BlankColorGradient = value;
    }

    private Gradient defaultColorGradient;

    public void SetBlankLineVisual()
    {
        defaultColorGradient = invalidColorGradient;
        invalidColorGradient = blankColorGradient;
    }

    public void SetDefaultColorGradient()
    {
        invalidColorGradient = defaultColorGradient;
    }
}
