using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConsoleView : MonoBehaviour
{
    [SerializeField] private TMP_Text _ffr;
    [SerializeField] private TMP_Text _type;
    [SerializeField] private TMP_Text _fps;
    [SerializeField] private TMP_Text _info;

    private void Start()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        _ffr.text = "FFR " + ConsoleController.FoveationLevel.ToString();
        _type.text = ConsoleController.Type.ToString();
        _fps.text = ConsoleController.frameRate.ToString() + " FPS";
        _info.text = ConsoleController.ZoneInfo;
    }
}
