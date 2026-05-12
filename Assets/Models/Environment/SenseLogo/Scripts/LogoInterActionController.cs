using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LogoInterActionController : MonoBehaviour
{
    private LogoViewBase _viewLogic;

    [Header("Events:")]
    public UnityEvent OnPointerEnter = new UnityEvent();
    public UnityEvent OnPointerExit = new UnityEvent();
    public UnityEvent OnPointerStay = new UnityEvent();

    private void Awake()
    {
        _viewLogic = GetComponent<LogoViewBase>();
        OnPointerEnter.AddListener(_viewLogic.Enter);
        OnPointerExit.AddListener(_viewLogic.Exit);
        OnPointerStay.AddListener(_viewLogic.Stay);
    }
    public void EnterState() => OnPointerEnter.Invoke();
    public void ExitState() => OnPointerExit.Invoke();
    public void StayState() => OnPointerStay.Invoke();
} 
