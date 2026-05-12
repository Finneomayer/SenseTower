using UnityEngine;

public class WorldCanvasUpdater : MonoBehaviour
{
    [SerializeField]
    private Canvas Canvas;

    private void Update()
    {
        Camera currentMainCamera = Camera.main;
        if (Canvas.worldCamera != currentMainCamera)
        {
            Canvas.worldCamera = currentMainCamera;
        }
    }
}
