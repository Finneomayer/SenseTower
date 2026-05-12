using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EmojiButton
{
    public EmojiType Type;
    public Button Button;
}

public class EmojiSwitcher : MonoBehaviour
{
    [SerializeField] private List<EmojiButton> _buttons;

    public event Action<EmojiType> OnPressEmoji;

    public void ShowEmojiButtons()
    {
        foreach (var button in _buttons)
        {
            button.Button.gameObject.SetActive(true);
        }
    }

    public void HideEmojiButtons()
    {
        foreach (var button in _buttons)
        {
            button.Button.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        foreach (var button in _buttons)
        {
            button.Button.onClick.AddListener(() => EmojiOnPressed(button.Type));
        }
    }

    private void EmojiOnPressed(EmojiType type)
    {
        OnPressEmoji?.Invoke(type);
    }

    private void OnDestroy()
    {
        foreach (var button in _buttons)
        {
            button.Button.onClick.RemoveAllListeners();
        }
    }
}
