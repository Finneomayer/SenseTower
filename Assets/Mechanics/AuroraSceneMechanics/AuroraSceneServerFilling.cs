using System;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Mechanics.AuroraSceneMechanics
{
    public class AuroraSceneServerFilling : NetworkBehaviour
    {
        [SerializeField] private GameObject _fasadChangerPrefab;
        private NetworkObjectReference _fasadNetworkObjectReference;


        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                if (IsAuroraSceneServer())
                {
                    CreateFasadChangingSystem();

                }
            }
            else
            {
                GetObjectsServerRpc();
            }
        }

        private bool IsAuroraSceneServer()
        {
            return Environment.GetEnvironmentVariable("RemoteFolderKey") == "StarSky";
        }

        private void CreatePillows()
        {
            //var pillow1 = Instantiate(_smallPillowPrefab, new Vector3(3.636f, 0.343f, 11.105f), Quaternion.Euler(-122.437f, -90f, 0));
            //if (pillow1.TryGetComponent(out NetworkObject networkObject1))
            //{
            //    networkObject1.Spawn();
            //    _pillowNetworkObjectReference1 = networkObject1;
            //}
//
            //var pillow2 = Instantiate(_smallPillowPrefab, new Vector3(3.636f, 0.343f, 11.83501f), Quaternion.Euler(-122.437f, -90f, 0));
            //if (pillow2.TryGetComponent(out NetworkObject networkObject2))
            //{
            //    networkObject2.Spawn();
            //    _pillowNetworkObjectReference2 = networkObject2;
            //}
//
            //var pillow3 = Instantiate(_smallPillowPrefab, new Vector3(3.872055f, 0.3840027f, 11.83501f), Quaternion.Euler(-122.437f, -90f, 0));
            //pillow3.transform.localScale = Vector3.one;
            //if (pillow3.TryGetComponent(out NetworkObject networkObject3))
            //{
            //    networkObject3.Spawn();
            //    _pillowNetworkObjectReference3 = networkObject3;
            //}
//
            //var pillow4 = Instantiate(_smallPillowPrefab, new Vector3(3.872055f, 0.3840027f, 11.111f), Quaternion.Euler(-122.437f, -90f, 0));
            //pillow4.transform.localScale = Vector3.one;
            //if (pillow4.TryGetComponent(out NetworkObject networkObject4))
            //{
            //    networkObject4.Spawn();
            //    _pillowNetworkObjectReference4 = networkObject4;
            //}
        }

        private void CreateFasadChangingSystem()
        {
            var fasadChanger = Instantiate(_fasadChangerPrefab, Vector3.zero, Quaternion.identity);
            if (fasadChanger.TryGetComponent(out NetworkObject networkObject))
            {
                networkObject.Spawn();
                _fasadNetworkObjectReference = networkObject;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void GetObjectsServerRpc()
        {
            if (!IsAuroraSceneServer())
            {
                return;
            }
            GetFasadClientRpc(_fasadNetworkObjectReference);
        }

        

        [ClientRpc]
        private void GetFasadClientRpc(NetworkObjectReference fasadReference)
        {

            if (fasadReference.TryGet(out NetworkObject networkObject2))
            {
                if (networkObject2.gameObject.GetComponent<FasadChangingSystem>() != null)
                {
                    var fasadChangingSystem = networkObject2.gameObject.GetComponent<FasadChangingSystem>();
                    
                    if (fasadChangingSystem != null) fasadChangingSystem.Init();
                }
            }
        }
    }
}
