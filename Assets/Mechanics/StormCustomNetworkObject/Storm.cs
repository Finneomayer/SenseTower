using System.Collections;
using UnityEngine;

namespace Assets.Mechanics.CustomMehanics
{
    public class Storm : MonoBehaviour
    {
        [SerializeField]
        private float MinLightningDelay = 7;
        [SerializeField]
        private float MaxLightningDelay = 50;
        [SerializeField]
        private float ThunderDelay = 2;
        [SerializeField]
        private float LightningDuration = 1;
        [SerializeField]
        private float MaxLightIntensity = 100000;

        private Transform _minPoint;
        private Transform _maxPoint;
        private Light _light;
        private Transform _lightningModel;
        private Material _lightningMaterial;
        private AudioSource _thunderSound;

        private void Awake()
        {
            foreach (Transform child in transform)
            {
                if (child.name == "MinPoint")
                {
                    _minPoint = child;
                }
                else if (child.name == "MaxPoint")
                {
                    _maxPoint = child;
                }
                else if (child.name == "Light")
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

        private void Start()
        {
            StartCoroutine(GeneratingThunderboltRoutine());
        }

        private IEnumerator GeneratingThunderboltRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(MinLightningDelay, MaxLightningDelay));

                float x = Random.Range(_minPoint.position.x, _maxPoint.position.x);
                float y = Random.Range(_minPoint.position.y, _maxPoint.position.y);
                float z = Random.Range(_minPoint.position.z, _maxPoint.position.z);
                SendThunderboltClientRpc(new Vector3(x, y, z));
            }

        }

        private void SendThunderboltClientRpc(Vector3 position)
        {
            _lightningModel.position = position;

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

            Color initialColor = RenderSettings.ambientSkyColor;
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

                RenderSettings.ambientSkyColor = Color.Lerp(RenderSettings.ambientSkyColor, targetColor, Time.deltaTime / timeLeft);
                timeLeft -= Time.deltaTime;
            }

            RenderSettings.ambientSkyColor = targetColor;
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

        private IEnumerator SetIntensitySmoothlyRoutine(Light light, float targetIntensity, float duration)
        {
            if (light.intensity == targetIntensity)
            {
                yield break;
            }

            float timeLeft = duration;
            while (timeLeft > 0)
            {
                yield return null;

                light.intensity = Mathf.Lerp(light.intensity, targetIntensity, Time.deltaTime / timeLeft);
                timeLeft -= Time.deltaTime;
            }

            light.intensity = targetIntensity;
        }
    }
}
