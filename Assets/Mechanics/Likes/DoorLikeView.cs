using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class DoorLikeView : MonoBehaviour
{
    [SerializeField] private DoorPlayerSensor _sensor; //hall
    [SerializeField] private CanvasGroup _likeCanvasGroup;
    [SerializeField] private TMP_Text _likeCountText;
    [SerializeField] private GameObject _stars;
    [SerializeField] private Image[] _starsSymbols = new Image[5];

    private Coroutine _alphaCoroutine;

    private void OnEnable()
    {
        _sensor.OnDoorNearEnter += _sensor_OnDoorNearEnter;
        _sensor.OnDoorNearExit += _sensor_OnDoorNearExit;
    }

    private void OnDisable()
    {
        _sensor.OnDoorNearEnter -= _sensor_OnDoorNearEnter;
        _sensor.OnDoorNearExit -= _sensor_OnDoorNearExit;
    }

    public void SetLikesCount(int likes)
    {
        _likeCountText.text = likes.ToString();
    }

    private void Start()
    {
        //SetStarsCount(Random.Range(0,6));
    }

    public void SetStarsCount(int stars)
    {
        if (stars >= 0)
        {
            _stars.SetActive(true);
            if (stars > _starsSymbols.Length)
            {
                stars = _starsSymbols.Length;
            }

            for (int i = 0; i < stars; i++)
            {
                _starsSymbols[i].color = Color.black;
            }
            for (int i = stars; i < _starsSymbols.Length; i++)
            {
                _starsSymbols[i].color = Color.white;
            }
        }
        else
        {
            _stars.SetActive(false);
        }
    }

    private void _sensor_OnDoorNearExit(Collider obj)
    {
        if (obj.GetComponentInParent<Camera>())
        {
            if (_alphaCoroutine != null) StopCoroutine(_alphaCoroutine);
            _alphaCoroutine = StartCoroutine(SetAlphaSmoothly(true));
        }
    }

    private void _sensor_OnDoorNearEnter(Collider obj) 
    {
        if (obj.GetComponentInParent<Camera>())
        {
            if (_alphaCoroutine != null) StopCoroutine(_alphaCoroutine);
            _alphaCoroutine = StartCoroutine(SetAlphaSmoothly(false));
        }
    }

    private IEnumerator SetAlphaSmoothly(bool toTransparent)
    {
        if (toTransparent)
        {
            float i = 1;
            while (i > 0)
            {
                SetAlphaToAllObjects(i);
                yield return new WaitForSeconds(0.05f);
                i = i - 0.1f;
            }
            SetAlphaToAllObjects(0f);
        }
        else
        {
            float i = 0;
            while (i < 1)
            {
                SetAlphaToAllObjects(i);
                yield return new WaitForSeconds(0.05f);
                i = i + 0.1f;
            }
            SetAlphaToAllObjects(1f);
        }        
    }

    private void SetAlphaToAllObjects(float value)
    {
        _likeCanvasGroup.alpha = value;
    }
}
