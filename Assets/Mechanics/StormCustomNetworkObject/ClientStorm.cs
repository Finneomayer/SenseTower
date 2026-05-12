using Mechanics.LoadSceneObjects;
using Mechanics.LoadSceneObjects.Models;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine;

namespace Assets.Mechanics.CustomMehanics
{
    public class ClientStorm : MonoBehaviour, INetworkCustomLogicService
    {
        [SerializeField]
        private float ThunderDelay = 2;
        [SerializeField]
        private float LightningDuration = 1;

        private Light _light;
        private Transform _lightningModel;
        private Material _lightningMaterial;
        private AudioSource _thunderSound;
        private Color? _initialAmbientLight;

        private CustomBehaviourNetworkObject _stormCustomNetworkObject;
        private CompositionRootNetworkScene _compositionRootNetworkScene;

        private void Awake()
        {
            foreach (Transform child in transform)
            {
                if (child.name == "Light")
                {
                    _light = child.GetComponent<Light>();
                    child.gameObject.SetActive(false);
                }
                else if (child.name == "LightningModel")
                {
                    _lightningModel = child;
                    if (child.TryGetComponent<MeshRenderer>(out var meshRenderer))
                    {
                        _lightningMaterial = meshRenderer.material;
                    }
                    child.gameObject.SetActive(false);
                }
                else if (child.name == "ThunderSound")
                {
                    child.TryGetComponent(out _thunderSound);
                }
            }
        }

        private void OnEnable()
        {
            if (_stormCustomNetworkObject != null)
            {
                _stormCustomNetworkObject.StateChanged -= OnStateChanged;
                _stormCustomNetworkObject.StateChanged += OnStateChanged;
            }
        }

        private void OnDisable()
        {
            if (_stormCustomNetworkObject != null)
            {
                _stormCustomNetworkObject.StateChanged -= OnStateChanged;
            }

            if (_initialAmbientLight.HasValue)
            {
                RenderSettings.ambientLight = _initialAmbientLight.Value;
            }
        }

        public void Init(StaticObject staticObject, CustomBehaviourNetworkObject customBehaviourNetworkObject)
        {
            if (_stormCustomNetworkObject != null)
            {
                _stormCustomNetworkObject.StateChanged -= OnStateChanged;
            }

            _stormCustomNetworkObject = customBehaviourNetworkObject;

            if (_stormCustomNetworkObject != null)
            {
                _stormCustomNetworkObject.StateChanged += OnStateChanged;
            }
        }

        private void OnStateChanged()
        {
            string currentState = _stormCustomNetworkObject.GetState();
            if (string.IsNullOrEmpty(currentState))
            {
                return;
            }

            Vector3Simple newPosition;
            try
            {
                newPosition = JsonConvert.DeserializeObject<Vector3Simple>(currentState);
            }
            catch (System.Exception)
            {
                Debug.Log($"no state for {gameObject.name}");
                return;
            }
           
            if (_compositionRootNetworkScene == null)
            {
                _compositionRootNetworkScene = FindObjectOfType<CompositionRootNetworkScene>();
                if (_compositionRootNetworkScene != null)
                {
                    _initialAmbientLight = _compositionRootNetworkScene.InitialLightingSettings.AmbientLight;
                }
            }

            _lightningModel.position = new Vector3(newPosition.x, newPosition.y, newPosition.z);

            StartCoroutine(LightningChangingMaterialRoutine());
            StartCoroutine(LightChangingRoutine());
            StartCoroutine(LightIntensityChangingRoutine());
            StartCoroutine(PlaySoundAfterTimeRoutine());
        }

        private IEnumerator PlaySoundAfterTimeRoutine()
        {
            yield return new WaitForSeconds(ThunderDelay);
            if (_thunderSound.isPlaying)
            {
                _thunderSound.Stop();
            }
            _thunderSound.pitch = Random.Range(0.7f, 1.2f);
            _thunderSound.Play();
        }

        private IEnumerator LightningChangingMaterialRoutine()
        {
            float startLightY = _lightningModel.position.y + 10;
            float endLightY = _lightningModel.position.y - 10;

            float speed = (endLightY - startLightY) / LightningDuration;

            _light.transform.position = new Vector3(_lightningModel.position.x, startLightY, _lightningModel.position.z);
            _light.transform.position += -0.1f * _lightningModel.forward;
            _light.gameObject.SetActive(true);

            while (_light.transform.position.y > endLightY)
            {
                yield return null;
                Vector3 currentPosition = _light.transform.position;
                currentPosition.y += speed * Time.deltaTime;

                _light.transform.position = currentPosition;
            }
            _light.gameObject.SetActive(false);
        }

        private IEnumerator LightChangingRoutine()
        {
            Color targetColor = _lightningMaterial.color;
            targetColor.a = 1;

            Color color = _lightningMaterial.color;
            color.a = 0;
            _lightningMaterial.color = color;

            _lightningModel.gameObject.SetActive(true);

            yield return SetMaterialColorSmoothlyRoutine(_lightningMaterial, targetColor, 0.1f * LightningDuration);

            targetColor.a = 0;
            yield return SetMaterialColorSmoothlyRoutine(_lightningMaterial, targetColor, 0.9f * LightningDuration);

            _lightningModel.gameObject.SetActive(false);
        }

        private IEnumerator LightIntensityChangingRoutine()
        {
            const float MaxIntensity = 2;

            Color initialColor = _initialAmbientLight.HasValue ? _initialAmbientLight.Value : RenderSettings.ambientLight;
            Color targetColor = initialColor * Mathf.Pow(2, MaxIntensity);
            yield return SetAmbientSkyColorSmoothlyRoutine(targetColor, 0.2f * LightningDuration);
            yield return SetAmbientSkyColorSmoothlyRoutine(initialColor, 0.8f * LightningDuration);
        }

        private IEnumerator SetAmbientSkyColorSmoothlyRoutine(Color targetColor, float duration)
        {
            float timeLeft = duration;
            while (timeLeft > 0)
            {
                yield return null;

                RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, targetColor, Time.deltaTime / timeLeft);
                timeLeft -= Time.deltaTime;
            }

            RenderSettings.ambientLight = targetColor;
        }

        private IEnumerator SetMaterialColorSmoothlyRoutine(Material material, Color targetColor, float duration)
        {
            if (material.color == targetColor)
            {
                yield break;
            }

            float timeLeft = duration;
            while (timeLeft > 0)
            {
                yield return null;

                material.color = Color.Lerp(material.color, targetColor, Time.deltaTime / timeLeft);
                timeLeft -= Time.deltaTime;
            }

            material.color = targetColor;
        }
    }
}
