using System;
using System.Collections.Generic;
using Assets.Mechanics.CustomMehanics;
using Assets.Mechanics.NetworkInteraction;
using Assets.Mechanics.NetworkInteraction.Services;
using Assets.Scripts.Data;
using Cysharp.Threading.Tasks;
using Data;
using Mechanics.LoadSceneObjects.Models;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Mechanics.LoadSceneObjects
{
    public class MovableSceneObject : AddressablesSceneObject
    {
        public async void ConstructMovableObject(StaticObject staticObject, NetworkObject networkObject,
            IGrabInteraction grabInteraction)
        {
            await UniTask.WaitUntil(() => _isObjectInit);
            spawnObject.transform.localScale = staticObject.Vectors.Scale.VectorComponentsToVector3();
            spawnObject.layer = LayerMask.NameToLayer("Grabable Distance");
            for (int i = 0; i < spawnObject.transform.childCount; i++)
            {
                spawnObject.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Grabable Distance");
            }

            NetworkXrGrab xrGrab = spawnObject.AddComponent<NetworkXrGrab>();
            xrGrab.interactionLayers = InteractionLayerMask.NameToLayer("Everything");
            xrGrab.useDynamicAttach = true;
            xrGrab.matchAttachPosition = true;
            xrGrab.matchAttachRotation = true;
            xrGrab.throwOnDetach = true;

            xrGrab.Init(networkObject, grabInteraction);
            ConstructModel(staticObject);

            if (networkObject.TryGetComponent(out NetworkItemType networkItemType))
            {
                networkItemType.SetAimObject(spawnObject.transform);
            }

            if (networkObject.TryGetComponent(out CustomBehaviourNetworkObject customBehaviour))
            {
                if (spawnObject.TryGetComponent(out INetworkCustomLogicService networkCustomLogicService))
                {
                    networkCustomLogicService.Init(staticObject, customBehaviour);
                }

                StaticObjectCollider[] colliders = spawnObject.GetComponentsInChildren<StaticObjectCollider>();

                foreach (var item in colliders)
                {
                    item.Init(staticObject);
                }
            }
        }

        private async void ConstructModel(StaticObject staticObject)
        {
            List<SkinComponent> movableObjectSkins = TryConvert(staticObject.Data);

            foreach (SkinComponent objectSkin in movableObjectSkins)
            {
                GameObject foundGameObject = null;
                if (spawnObject.name.Contains(objectSkin.ComponentPath))
                {
                    foundGameObject = spawnObject;
                }
                else
                {
                    foreach (Transform childTransform in spawnObject.transform)
                    {
                        if (spawnObject.name.Contains(objectSkin.ComponentPath))
                        {
                            foundGameObject = childTransform.gameObject;
                            break;
                        }
                    }
                }

                if (foundGameObject == null)
                    continue;

                if (objectSkin.ComponentType == Enumenators.SkinType.Material)
                {
                    Material material = await _factory.LoadObject<Material>(objectSkin.ComponentName);
                    if (foundGameObject.TryGetComponent(out MeshRenderer meshRenderer))
                    {
                        meshRenderer.material = material;
                    }
                }

                if (objectSkin.ComponentType == Enumenators.SkinType.Mesh)
                {
                    Mesh mesh = await _factory.LoadObject<Mesh>(objectSkin.ComponentName);
                    if (foundGameObject.TryGetComponent(out MeshFilter meshFilter))
                    {
                        meshFilter.mesh = mesh;
                    }
                }

                if (objectSkin.ComponentType == Enumenators.SkinType.Script)
                {
                    Type scriptType = Type.GetType(objectSkin.ComponentName);
                    foundGameObject.AddComponent(scriptType);
                }
            }
        }

        private List<SkinComponent> TryConvert(Dictionary<string,string> data)
        {
            List<SkinComponent> movableObjectSkins = new();

            foreach (KeyValuePair<string,string> dataValue in data)
            {
                string[] splitString = dataValue.Key.Split('_');

                if (splitString.Length > 0)
                {
                    if (splitString[0].Equals("ComponentType") && splitString[2].Equals("Path"))
                    {
                        movableObjectSkins.Add(new SkinComponent()
                        {
                            ComponentPath = splitString[3],
                            ComponentType = Enum.Parse<Enumenators.SkinType>(splitString[1]),
                            ComponentName = dataValue.Value
                        });
                    }
                }
            }
            return movableObjectSkins;
        }
    }
}