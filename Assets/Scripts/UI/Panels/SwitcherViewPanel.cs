using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Space;
using UI;
using UnityEngine;
using Zenject;

public class SwitcherViewPanel : ViewPanel
{
    #region Inspector

    [SerializeField] private UISwitcherService _switcherService;
    private ISpaceModeData _spaceModeData;

    #endregion
    
    [Inject]
    public void Construct(ISpaceModeData spaceModeData)
    {
        _spaceModeData = spaceModeData;
    }

    public override void ShowPanel()
    {
        base.ShowPanel();
        
    }
}
