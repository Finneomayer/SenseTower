using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteSceneInfrastructure : MonoBehaviour
{
    #region Inspector
    [SerializeField] private Material _skybox;
    [SerializeField] private List<GameObject> _teleportObjects = new();
    [SerializeField] private List<GameObject> _chairObjects = new();
    #endregion

    public List<GameObject> ObjectsForTeleport
    {
        get => _teleportObjects;
        private set => _teleportObjects = value;
    }
    
    public List<GameObject> ChairObjects
    {
        get => _chairObjects;
        private set => _chairObjects = value;
    }

    public Material SkyBoxMaterail {
        get => _skybox;
        private set => _skybox = value;

    }
}
