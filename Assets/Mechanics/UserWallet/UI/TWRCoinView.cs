using System;
using Mechanics.UserWallet;
using Mechanics.UserWallet.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TWRCoinView : MonoBehaviour
{
    #region Inspector

    [SerializeField] private CoinInfrastructure _coinInfrastructure;
    [SerializeField] private TMP_Text _tittleValue;
    [SerializeField] private RadialButton _button;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private Image _radialImage;
    public int CoinValue => _coinValue;

    #endregion

    private bool _remoteControl;
    private int _coinValue = 1;
    private float _previousNormalizedValue;
    
    private void Start()
    {
        _radialImage.fillAmount = 0.01f;
        FillValue(true);
    }

    private void Update()
    {
        if (_remoteControl) return;
        
        if (_button.Dragged && Math.Abs((_button.NormalizedValue - _previousNormalizedValue)) > 0.001f)
            InteractWithAudioSource(true);
        else
            InteractWithAudioSource(false);

        if (_button.Dragged)
        {
            SetCoinViewInfo();
        }
    }

    public void SetCoinValue(int value, bool checkWallet)
    {
        _coinValue = value;
        _remoteControl = true;
        float normalizedValue = (float)value / 100;
        FillImage(normalizedValue, checkWallet);
        InteractWithAudioSource(false);
        FillValue(checkWallet);
    }

    public void DisableRemoteControl()
    {
        _remoteControl = false;
        SetCoinViewInfo();
    }

    private void SetCoinViewInfo()
    {
        _coinValue = (int) Math.Round(_button.NormalizedValue * 100);
        _previousNormalizedValue = _button.NormalizedValue;
        FillImage(_button.NormalizedValue, true);
        FillValue(true);
    }

    private void FillImage(float value, bool checkWallet)
    {
        _radialImage.fillAmount = value;

        if (checkWallet && _coinInfrastructure.GetWalletValue() == 0)
            _radialImage.fillAmount = 0;
    }

    private void FillValue(bool checkWallet)
    {
        int result = CoinValue;

        if (checkWallet && result > _coinInfrastructure.GetWalletValue())
        {
            result = _coinInfrastructure.GetWalletValue();
        }

        _tittleValue.text = result.ToString();
    }

    private void InteractWithAudioSource(bool play)
    {
        if (play)
        {
            if (!_audio.isPlaying)
                _audio.Play();
        }
        else
        {
            _audio.Stop();
        }
    }
}