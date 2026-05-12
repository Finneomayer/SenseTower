using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class TextRotation : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<int> _switchedTextNumber;
    [SerializeField] private Canvas[] _textCanvases;
    [SerializeField] private Button[] _buttons;
    private bool _isRotating;

    private void SetData()
    {       
        var buttonContainer = GameObject.Find("Buttons");
        if (buttonContainer != null)
        {
            buttonContainer.AddComponent<TrackedDeviceGraphicRaycaster>();
            _buttons = buttonContainer.GetComponentsInChildren<Button>();
            Debug.LogWarning(_buttons.Length);
        }

        var canvasContainer = GameObject.Find("Text");
        if (canvasContainer != null)
        {
            _textCanvases = canvasContainer.GetComponentsInChildren<Canvas>();
        }

        if (buttonContainer != null && canvasContainer != null)
        {
            _isRotating = true;

            for (int i = 0; i < _buttons.Length; i++)
            {
                int buttonNum = i;
                _buttons[i].onClick.AddListener(() => ButtonOnClick(buttonNum));
            }

            _switchedTextNumber.OnValueChanged += SwitchButtons;
            _switchedTextNumber.OnValueChanged += SwitchTexts;

#if !UNITY_SERVER
            SwitchButtonsAtStart();
            SwitchTextsAtStart();
#endif
        }
    }

    private void FixedUpdate()
    {
        if (_isRotating)
        {
            if (_switchedTextNumber.Value != 0)
            {                
                _textCanvases[_switchedTextNumber.Value - 1].transform.Rotate(0, 0.3f, 0);
            }
        }
    }

    private void OnDisable()
    {
        _switchedTextNumber.OnValueChanged -= SwitchButtons;
        _switchedTextNumber.OnValueChanged -= SwitchTexts;
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].onClick.RemoveAllListeners();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SetData();

    }

    private void SwitchButtonsAtStart()
    {
        if (_switchedTextNumber.Value != 0) 
        {
            _buttons[_switchedTextNumber.Value - 1].colors = ChangeColor(_buttons[_switchedTextNumber.Value - 1].colors, Color.red);
        }
    }

    private void SwitchTextsAtStart()
    {
        for (int i = 0; i < _textCanvases.Length; i++)
        {
            _textCanvases[i].gameObject.SetActive(false);
        }
        if (_switchedTextNumber.Value != 0)
        {
            _textCanvases[_switchedTextNumber.Value - 1].gameObject.SetActive(true);
        }
    }

    private void SwitchButtons(int oldv, int newv)
    {
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].colors = ChangeColor(_buttons[i].colors, Color.white);
        }
        if (newv != 0)
        {
            _buttons[newv - 1].colors = ChangeColor(_buttons[newv - 1].colors, Color.red);
        }
    }

    private void SwitchTexts(int oldv, int newv)
    {
        for (int i = 0; i < _textCanvases.Length; i++)
        {
            _textCanvases[i].gameObject.SetActive(false);
        }
        if (newv != 0)
        {
            _textCanvases[newv - 1].gameObject.SetActive(true);
        }
    }

    private ColorBlock ChangeColor(ColorBlock colorBlock, Color color)
    {
        ColorBlock newColorBlock = colorBlock;
        newColorBlock.normalColor = color;
        newColorBlock.selectedColor = color;
        return newColorBlock;
    }

    private void ButtonOnClick(int i)
    {
        if (i + 1 == _switchedTextNumber.Value) 
        {
            SetButtons(0);            
        } 
        else SetButtons(i + 1);
    }

    private void SetButtons(int num)
    {
        SetButtonsServerRPC(num);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetButtonsServerRPC(int num)
    {
        _switchedTextNumber.Value = num;
    }
}
