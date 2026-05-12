using Mechanics.LoadSceneObjects;
using Mechanics.LoadSceneObjects.Models;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine;

namespace Assets.Mechanics.CustomMehanics
{

    public class ServerStormGenerator : MonoBehaviour, INetworkCustomLogicService
    {
        [SerializeField]
        private float MinLightningDelay = 7;
        [SerializeField]
        private float MaxLightningDelay = 50;
        [SerializeField]
        private Transform _minPoint;
        [SerializeField]
        private Transform _maxPoint;

        private CustomBehaviourNetworkObject _stormCustomNetworkObject;

        public void Init(StaticObject staticObject, CustomBehaviourNetworkObject customBehaviourNetworkObject)
        {
            _stormCustomNetworkObject = customBehaviourNetworkObject;
            StartCoroutine(GeneratingThunderboltRoutine());
        }

        private IEnumerator GeneratingThunderboltRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(MinLightningDelay, MaxLightningDelay));
                Vector3Simple newPosition;
                newPosition.x = Random.Range(_minPoint.position.x, _maxPoint.position.x);
                newPosition.y = Random.Range(_minPoint.position.y, _maxPoint.position.y);
                newPosition.z = Random.Range(_minPoint.position.z, _maxPoint.position.z);

                if (_stormCustomNetworkObject != null)
                {
                    string jsonString = JsonConvert.SerializeObject(newPosition);
                    _stormCustomNetworkObject.SetStateServer(jsonString);
                    _stormCustomNetworkObject.SetStateWithoutNotificationServer("");
                }
            }
        }
    }
}
