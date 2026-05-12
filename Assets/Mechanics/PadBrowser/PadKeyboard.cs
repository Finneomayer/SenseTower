using System;
using Assets.Mechanics.Keyboard.Scripts;
using Assets.Mechanics.PadKeyboard;
using Assets.Scripts.Zones;
using UI;
using UnityEngine;

public class PadKeyboard : MonoBehaviour
{
    [SerializeField] private Browser _browser; //used for stationary pad keyboard to connect to main browser
    [SerializeField] private ViewPanel _searchPanel;
    [SerializeField] private PhysicKeyboard _keyboard;
    [SerializeField] private PadCollider _padCollider;
    [SerializeField] private Transform _anchorOnTable;
    [SerializeField] private Transform _anchorFree;

    private float _tableYcoordinate;
    private static bool _keyboardIsOnTable;
    private float _padStartScale;

    //only for NetworkPad
    private PresentationPad _presentationPad;

    private Camera _camera;
    
    /// <summary>
    /// For local Pad
    /// </summary>
    public void InitKeyboardOfLocalPad()
    {
        _padCollider.PadTableZoneEnter += PadCollider_PadTableZoneEnter;
        _padCollider.PadTableZoneExit += PadCollider_PadTableZoneExit;
    }

    private void HideKeyboard()
    {
        _keyboard.CloseKeyboard();
    }

    private void OpenKeyboard()
    {        
        _keyboard.OpenKeyboard();
    }

    public void UnsubscribeKeyboard()
    {
        PadCollider_PadTableZoneExit();
    }

    private void Start()
    {
        _padStartScale = _padCollider.transform.localScale.x;

        //For local pad
        if (_browser != null) _browser.CanvasKeyboard.SetSecondaryKeyboard(_keyboard);

        transform.parent = transform.root;

        //For network Pad
        if (TryGetComponent<PresentationPad>(out _presentationPad))
        {
            InitKeyboardOfNetworkPad();
        }

        _searchPanel.PanelShown += OpenKeyboard; //works on local & network pads
        _searchPanel.PanelHidden += HideKeyboard; //works on local & network pads

        HideKeyboard();
    }

    private void OnDestroy()
    {
        _keyboardIsOnTable = false;
    }

    private void InitKeyboardOfNetworkPad()
    {
        _padCollider.PadTableZoneEnter += PadCollider_PadTableZoneEnter;
        _padCollider.PadTableZoneExit += PadCollider_PadTableZoneExit;
    }

    public void SetBrowser(Browser browser)
    {
        _browser = browser;
        if (_browser != null) _browser.CanvasKeyboard.SetSecondaryKeyboard(_keyboard);

    }
    
#if !UNITY_SERVER
    private void Update()
    {     
        if (_keyboardIsOnTable) KeyboardOnTableBehaviour();
        else KeyboardPadDistanceControl();
    }
#endif

    private void PadCollider_PadTableZoneEnter(Transform tableHeight)
    {
        _tableYcoordinate = tableHeight.position.y;
        _keyboardIsOnTable = true;
    }

    private void PadCollider_PadTableZoneExit()
    {
        _keyboardIsOnTable = false;
    }

    private void KeyboardOnTableBehaviour()
    {
        if (_camera == null) 
            _camera = Camera.main;

        #region rotation
        //direction from the tablet to the user

        Vector3 deltaVector = Vector3.zero;
        if (_camera != null) deltaVector = _padCollider.transform.position - _camera.transform.position; 
        deltaVector.y = 0;
        float playerDistance = deltaVector.magnitude;

        //I calculate the angle between [the rotation of the tablet] and [the direction from the tablet to the user] 
        float padPlayerAngle = _padCollider.transform.rotation.eulerAngles.y - 180 + Vector3.SignedAngle(deltaVector, Vector3.back, Vector3.up);

        float keyboardAngle;
        //the angle of the keyboard is equal to either the angle of the tablet, or the angle of the direction vector towards the user
        if (padPlayerAngle > -60 && padPlayerAngle < 60 || playerDistance > 1f) keyboardAngle = _padCollider.transform.eulerAngles.y;
        else keyboardAngle = -Vector3.SignedAngle(deltaVector, Vector3.forward, Vector3.up);
        
        _keyboard.transform.rotation = Quaternion.Slerp(_keyboard.transform.rotation, Quaternion.Euler(90, keyboardAngle, 0), 0.1f);
        #endregion

        #region moving

        Vector3 keypboardPos = _anchorOnTable.transform.position;
        keypboardPos.y = _tableYcoordinate;
        _keyboard.transform.position = keypboardPos;

        #endregion
    }

    private void KeyboardPadDistanceControl()
    {
        _keyboard.transform.position = _anchorFree.transform.parent.TransformPoint(new Vector3(
            _anchorFree.transform.localPosition.x,
            _anchorFree.transform.localPosition.y - (_padStartScale - _padCollider.transform.localScale.y) * 0.04f,
            _anchorFree.transform.localPosition.z));

        _keyboard.transform.rotation = _anchorFree.transform.rotation;
    }
}
