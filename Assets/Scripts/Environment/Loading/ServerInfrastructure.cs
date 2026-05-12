using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerInfrastructure : MonoBehaviour
{
    #region Inspector
    [SerializeField] private List<GameObject> _chairObjects = new();
    [SerializeField] private List<Vector3> _chairPosition = new();

    #endregion
    public List<GameObject> ChairObjects
    {
        get => _chairObjects;
        private set => _chairObjects = value;
    }
    public List<Vector3> ChairPositions
    {
        get => _chairPosition;
        private set => _chairPosition = value;
    }
}
