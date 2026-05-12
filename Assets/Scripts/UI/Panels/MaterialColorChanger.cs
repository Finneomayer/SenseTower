using UnityEngine;
using System.Collections;

namespace UI
{
    [RequireComponent(typeof(MeshRenderer))]
    public class MaterialColorChanger : MonoBehaviour
    {
        [SerializeField]
        private float _changeDuration = 0.1f;

        private Coroutine _changingColorRoutine;

        private Material _material;

        private void Awake()
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            _material = meshRenderer.material;
        }

        public void SetAlphaSmoothly(float targetAlpha)
        {
            if (_material == null)
            {
                return;
            }
            Color targetColor = _material.color;
            targetColor.a = targetAlpha;
            SetColorSmoothly(targetColor);
        }

        public void SetAlphaInstantly(float targetAlpha)
        {
            if (_material == null)
            {
                return;
            }
            Color targetColor = _material.color;
            targetColor.a = targetAlpha;
            _material.color = targetColor;
        }

        public void SetColorSmoothly(Color targetColor)
        {
            if (_changingColorRoutine != null)
            {
                StopCoroutine(_changingColorRoutine);
            }
            _changingColorRoutine = StartCoroutine(SetColorSmoothlyRoutine(targetColor));
        }

        public void SetColorInstantly(Color targetColor)
        {
            if (_changingColorRoutine != null)
            {
                StopCoroutine(_changingColorRoutine);
                _changingColorRoutine = null;
            }
            if (_material == null)
            {
                return;
            }
            _material.color = targetColor;
        }

        private IEnumerator SetColorSmoothlyRoutine(Color targetColor)
        {
            if (_material == null)
            {
                _changingColorRoutine = null;
                yield break;
            }

            if (_material.color == targetColor)
            {
                _changingColorRoutine = null;
                yield break;
            }

            float timeLeft = _changeDuration;
            while (timeLeft > 0)
            {
                yield return null;

                _material.color = Color.Lerp(_material.color, targetColor, Time.deltaTime / timeLeft);
                timeLeft -= Time.deltaTime;
            }

            _material.color = targetColor;
            _changingColorRoutine = null;
        }
    }
}
