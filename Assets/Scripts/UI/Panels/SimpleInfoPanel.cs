using UnityEngine;
using TMPro;

public class SimpleInfoPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject InfoContainer;
    [SerializeField]
    private TMP_Text InfoText;

    public void SetActivePanel(bool active)
    {
        InfoContainer.SetActive(active);
    }

    public void SetText(string text)
    {
        InfoText.text = text;
    }
}
