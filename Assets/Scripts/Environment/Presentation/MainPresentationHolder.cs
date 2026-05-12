using UnityEngine;

public class MainPresentationHolder : MonoBehaviour
{
    [field: SerializeField]
    public Presentation Presentation { get; private set; }

    [field: SerializeField]
    public PadSwitcher PadSwitcher { get; set; }

    [field: SerializeField]
    public bool CanConnectPad { get; private set; }

    public bool IsPadConnectionAvailable()
    {
        return CanConnectPad && PadSwitcher != null && Presentation != null && Presentation.transform.position.x != 1000;
    }
}
