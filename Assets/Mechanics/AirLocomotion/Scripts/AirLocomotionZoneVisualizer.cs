using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Mechanics.AirLocomotion.Scripts
{
    public class AirLocomotionZoneVisualizer : MonoBehaviour
    {
        [SerializeField] private MeshRenderer MeshRenderer;
        [SerializeField] private AirLocomotionZone AirLocomotionZone;
        [SerializeField] private AirLocomotionZoneVisualizerCollider VisualizerCollider;
        [SerializeField, Range(0f, 1f)] private float MinAlpha = 0;
        [SerializeField, Range(0f, 1f)] private float MaxAlpha = 0.2f;
        [SerializeField, Range(0f, 1f)] private float ChangeAlphaDurationInSec = 0.2f;

        private List<AirLocomotionZoneVisualizerActivator> _activators;

        private Material _material;
        private Coroutine _changingColorRoutine;

        private void Awake()
        {
            _activators = new();
            _material = MeshRenderer.material;

            VisualizerCollider.Init(AirLocomotionZone);
            SetActiveInstantly(false);
        }

        private void OnEnable()
        {
            VisualizerCollider.Entered += OnVisualizerColliderEntered;
            VisualizerCollider.Exited += OnVisualizerColliderExited;
        }

        private void OnDisable()
        {
            VisualizerCollider.Entered -= OnVisualizerColliderEntered;
            VisualizerCollider.Exited -= OnVisualizerColliderExited;
        }

        public void ForseStopLocomotion()
        {
            foreach (AirLocomotionZoneVisualizerActivator activator in _activators)
            {
                if (activator.TryGetComponent(out AirLocomotionHand locomotionHand))
                {
                    locomotionHand.ForseStopLocomotion(AirLocomotionZone);
                }
            }
        }

        private void OnVisualizerColliderEntered(AirLocomotionZoneVisualizerActivator activator)
        {
            if (!_activators.Contains(activator))
            {
                activator.Disabled += OnActivatorDisabled;
                _activators.Add(activator);
                SetActiveVisualization(true);
            }
        }

        private void OnVisualizerColliderExited(AirLocomotionZoneVisualizerActivator activator)
        {
            activator.Disabled -= OnActivatorDisabled;
            _activators.Remove(activator);
            if (_activators.Count == 0)
            {
                SetActiveVisualization(false);
            }
        }

        private void OnActivatorDisabled(AirLocomotionZoneVisualizerActivator activator)
        {
            OnVisualizerColliderExited(activator);
        }

        private void SetActiveVisualization(bool active)
        {
            if (this == null || !gameObject.activeInHierarchy)
            {
                return;
            }

            if (_changingColorRoutine != null)
            {
                StopCoroutine(_changingColorRoutine);
            }

            _changingColorRoutine = StartCoroutine(SetActiveSmoothly(active));
        }

        private void SetActiveInstantly(bool active)
        {
            float targetAlpha = active ? MaxAlpha : MinAlpha;
            Color targetColor = _material.color;
            targetColor.a = targetAlpha;
            _material.color = targetColor;
        }

        private IEnumerator SetActiveSmoothly(bool active)
        {
            float targetAlpha = active ? MaxAlpha : MinAlpha;
            Color targetColor = _material.color;
            targetColor.a = targetAlpha;

            if (_material.color.a == targetAlpha)
            {
                _changingColorRoutine = null;
                yield break;
            }

            float timeLeft = ChangeAlphaDurationInSec;
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