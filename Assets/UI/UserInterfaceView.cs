using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceView : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputTextLogin;
    [SerializeField] private TMP_InputField _inputTextPassword;
    [SerializeField] private Button _buttonRegister;
    [SerializeField] private Button _buttonLogin;
    [SerializeField] private GameObject LoadingText;
    public event Action onPressedRegister;
    public event Action<string, string> onPressedLogin;


    private void Start()
    {
        Application.runInBackground = true;

        _buttonRegister.onClick.AddListener(() => onPressedRegister?.Invoke());

        _buttonLogin.onClick.AddListener(() => StartCoroutine(EnableLogicAfterTime(4f)));
    }
    
    private IEnumerator EnableLogicAfterTime(float time)
    {
        onPressedLogin?.Invoke(_inputTextLogin.text, _inputTextPassword.text);
        _buttonLogin.interactable = false;
        LoadingText.SetActive(true);
        yield return new WaitForSeconds(time);
        _buttonLogin.interactable = true;
        LoadingText.SetActive(false);
    }
}
