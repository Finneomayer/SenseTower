using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Localization;
using Assets.Mechanics.PadKeyboard;
using Meta.WitAi;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

public class RecorderUserView : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Button _recButton;
    [SerializeField] private Image _recButtonBack;
    [SerializeField] private Button _stopRecButton;
    [SerializeField] private Button _playButton;
    [SerializeField] private TMP_Text _durationText;
    [SerializeField] private Button _stopPlayButton;
    [SerializeField] private Button _pausePlayButton;
    [SerializeField] private LookAtPlayer _rotator;
    [SerializeField] private Animation _animation;
    [SerializeField] private AnimationClip _clipRecord;
    [SerializeField] private AnimationClip _clipRecordToIdle;
    [SerializeField] private AnimationClip _clipPlay;
    [SerializeField] private AnimationClip _clipPlayToIdle;
    [SerializeField] private TMP_Text _timerRec;
    [SerializeField] private TMP_Text _timerPlay;
    [SerializeField] private LocalizationVariant _waitDownloading;
    [SerializeField] private LocalizationVariant _waitSaving;
    [SerializeField] private GameObject _cone;
    [SerializeField] private GameObject _panelMain;
    [SerializeField] private GameObject _panelPlay;
    [SerializeField] private FingerPhysicButton[] _physButtons;
    [SerializeField] private MeshRenderer[] _meshButtons;
    [SerializeField] private Material _idleMaterial;
    [SerializeField] private Material _emptyMaterial;

    public event Action RequestToRecord;
    public event Action RequestToStopRec;
    public event Action RequestToPlay;
    public event Action RequestToStopPlay;
    public event Action RequestToPausePlay;
    private bool _isOwner;
    private bool _isPlaying;
    private Color _recButtonColorDefault;
    private Coroutine _buttonPauseBlockCoroutine;

    private void OnEnable()
    {
        _panelPlay.SetActive(false);
        _panelMain.SetActive(false);
    }

    public void InitAsOwner(Transform player)
    {
        _rotator.SetPlayer(player);
        _isOwner = true;
        _recButtonColorDefault = _recButtonBack.color;

        _recButton.onClick.AddListener(OnClickRec);
        _playButton.onClick.AddListener(OnClickPlay);
        _stopRecButton.onClick.AddListener(OnClickStopRec);
        _stopPlayButton.onClick.AddListener(OnClickStopPlay);
        _pausePlayButton.onClick.AddListener(OnClickPausePlay);

        //StartCoroutine(CheckMoving());

        _panelPlay.SetActive(false);
        _panelMain.SetActive(true);

        if (_physButtons != null && _physButtons.Length > 0)
        {
            foreach (var btn in _physButtons)
            {
                btn.OnPress += PhysButtonPressedOwner;
            }
        }
    }

    public void InitAsWatcher(Transform player)
    {
        _rotator.SetPlayer(player);
        _isOwner = false;

        _stopPlayButton.onClick.AddListener(OnClickStopPlay);
        _pausePlayButton.onClick.AddListener(OnClickPausePlay);

        foreach (var mesh in _meshButtons)
        {
            mesh.material = _emptyMaterial;
        }
    }

    private void PhysButtonPressedWatcher()
    {      
        if (!_isPlaying)
        {
            OnClickPlay();
            _canvas.enabled = true;
            _cone.SetActive(true);
        }
    }

    private void PhysButtonPressedOwner()
    {
        _canvas.enabled = true;
        _cone.SetActive(true);
    }    

    public void RecordExists(int length)
    {
        if (_isOwner)
        {
            SetPlayButton(true);
            SetDurationAfterRecord(length);
        }
        else
        {
            foreach (var mesh in _meshButtons)
            {
                mesh.material = _idleMaterial;
            }

            if (_physButtons != null && _physButtons.Length > 0)
            {
                foreach (var btn in _physButtons)
                {
                    btn.OnPress += PhysButtonPressedWatcher;
                }
            }
        }
    }

    public void SetDurationAfterRecord(int length)
    {
        length += 1;
        if (length < 10) _durationText.text = $"0{length / 60}:0{length % 60}";
        else _durationText.text = $"0{length / 60}:{length % 60}";
    }

    public void SetButtons(bool rec, bool stop, bool play)
    {
        SetRecButton(rec);
        SetStopRecButton(stop);
        SetPlayButton(play);
    }    

    public void SetButtons(bool rec, bool stop)
    {
        SetRecButton(rec);
        SetStopRecButton(stop);
    }

    public void SetTimerText(int seconds, int lengthInSeconds = 0, bool isRec = false)
    {
        if (seconds == 0)
        {
            _timerRec.text = "";
            _timerPlay.text = "";
            _pausePlayButton.gameObject.SetActive(false);
            _stopPlayButton.gameObject.SetActive(false);
            return;
        }

        if (isRec)
        {
            _timerRec.color = Color.red;
            if (seconds < 10) _timerRec.text = $"0{seconds / 60}:0{seconds % 60} (max 01:00)";
            else _timerRec.text = $"0{seconds / 60}:{seconds % 60} (max 01:00)";
        }
        else
        {
            _timerRec.color = Color.green;

            string timeFromStart;
            if (seconds < 10) timeFromStart = $"0{seconds / 60}:0{seconds % 60}";
            else timeFromStart = $"0{seconds / 60}:{seconds % 60}";

            string timeTillEnd;
            int secondsEnd = lengthInSeconds - seconds + 1;
            if (secondsEnd < 10) timeTillEnd = $"0{secondsEnd / 60}:0{secondsEnd % 60}";
            else timeTillEnd = $"0{secondsEnd / 60}:{secondsEnd % 60}";

            _timerPlay.text = $"{timeFromStart}  /  - {timeTillEnd}";
            _pausePlayButton.gameObject.SetActive(true);
            _stopPlayButton.gameObject.SetActive(true);

        }
    }

    public void NotifyToWaitWhileSaving()
    {
        _timerRec.text = _waitSaving.Localize();
    }

    public void NotifyToWaitWhileDownloading()
    {
        _timerPlay.text = _waitDownloading.Localize();
    }

    private void SetRecButton(bool active) => _recButton.interactable = active;

    private void SetPlayButton(bool active)
    {
        if (active) _playButton.gameObject.SetActive(true);
        _playButton.interactable = active;
    }

    private void SetStopRecButton(bool active) => _stopRecButton.interactable = active;

    public void SetIdleConeFromRec()
    {
        if (!_isOwner) return;
        _animation.clip = _clipRecordToIdle;
        _recButtonBack.color = _recButtonColorDefault;
        _animation.Play();
    }
    public void SetIdleConeFromPlay()
    {
        _animation.clip = _clipPlayToIdle;
        _animation.Play();
        _panelPlay.SetActive(false);

        if (!_isOwner) return;
        _panelMain.SetActive(true);
    }

    private void OnClickRec()
    {
        SetButtons(false, true, false);
        _animation.clip = _clipRecord;
        _animation.Play();
        _recButtonBack.color = Color.red;
        _durationText.text = "";
        RequestToRecord?.Invoke();
    }

    private void OnClickStopRec()
    {
        SetButtons(false, false, false); //everything false because waiting saving
        _animation.clip = _clipRecordToIdle;
        _animation.Play();
        _recButtonBack.color = _recButtonColorDefault;
        RequestToStopRec?.Invoke();
    }

    private void OnClickPlay()
    {
        SetButtons(false, false, false);
        _animation.clip = _clipPlay;
        _animation.Play();

        _panelPlay.SetActive(true);
        _panelMain.SetActive(false);
        RequestToPlay?.Invoke();

        _isPlaying = true;
    }

    public void ClickPlayExternal()
    {
        if (_isOwner)
        {
            SetButtons(false, false, false);
            _animation.clip = _clipPlay;
            _animation.Play();

            _canvas.enabled = true;
            _panelPlay.SetActive(true);
            _panelMain.SetActive(false);

            _isPlaying = true;
        }        
    }

    private void OnClickStopPlay()
    {
        RequestToStopPlay?.Invoke();
        _isPlaying = false;
    }

    public void ClickStopPlayExternal()
    {
        SetButtons(true, false, true);
        SetIdleConeFromPlay();
        SetTimerText(0);        
        _isPlaying = false;

        if (!_isOwner)
        {
            _cone.SetActive(false);
            _canvas.enabled = false;
        }
    }

    private void OnClickPausePlay()
    {
        RequestToPausePlay?.Invoke();
        if (_buttonPauseBlockCoroutine == null) _buttonPauseBlockCoroutine = StartCoroutine(DelayCoroutine(2, _pausePlayButton));
    }

    /// <summary>
    /// when user connects while pause to prevent start button working on !owner
    /// </summary>
    public void BlockPhysButton()
    {
        //_canvas.enabled = true;
        //_cone.SetActive(true);
        if (_isOwner) return;
        _isPlaying = true;
    }

    private IEnumerator DelayCoroutine(int secDelay, Button buttonBlocked)
    {
        buttonBlocked.interactable = false;
        yield return new WaitForSeconds(secDelay);
        buttonBlocked.interactable = true;
        _buttonPauseBlockCoroutine = null;
    }

    //private IEnumerator CheckMoving()
    //{
    //    Vector3 currentPosition = transform.position;
    //    Vector3 previousPosition;

    //    while (true)
    //    {
    //        previousPosition = currentPosition;
    //        currentPosition = transform.position;

    //        if (currentPosition == previousPosition)
    //        {
    //            _canvas.enabled = _isOwner;
    //            _cone.SetActive(_isOwner);
    //        }
    //        else
    //        {
    //            _canvas.enabled = false;
    //            _cone.SetActive(false);
    //        }

    //        yield return new WaitForSeconds(0.2f);
    //    }
    //}
}
