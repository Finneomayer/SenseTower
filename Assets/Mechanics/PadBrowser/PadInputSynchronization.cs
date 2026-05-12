using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PadSearch))]
public class PadInputSynchronization : MonoBehaviour
{
    [SerializeField] private BrowserAdminControlPanel _mainPanel;
    private PadSearch _padSearch;

    private void Awake()
    {
        if (_mainPanel == null)
        {
            _mainPanel = FindObjectOfType<BrowserAdminControlPanel>();
        }
        _padSearch = GetComponent<PadSearch>();
    }

    private void Start()
    {
        _padSearch.InputFieldSetFocused += PadSearch_InputSetFocused;
        _padSearch.PadBrowserSetFocused += PadSearch_PadBrowserSetFocused;
    }

    private void PadSearch_PadBrowserSetFocused()
    {
        _mainPanel.SetBrowserFocused();
    }

    private void FixedUpdate()
    {
        //if (_searchInput.isFocused || _mainPanel.IsInputFieldFocused())
        _padSearch.SyncrhronizeInputField(_mainPanel.GetInputFieldValue());
    }

    private void OnDisable()
    {
        _padSearch.InputFieldSetFocused -= PadSearch_InputSetFocused;
    }

    private void PadSearch_InputSetFocused()
    {
        _mainPanel.SetInputFieldFocused();
    }
}
