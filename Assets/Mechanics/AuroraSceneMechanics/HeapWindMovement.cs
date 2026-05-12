using System.Collections.Generic;
using Mechanics.LoadSceneObjects;
using Mechanics.LoadSceneObjects.Models;
using UnityEngine;

public class HeapWindMovement : MonoBehaviour, INetworkCustomLogicService
{
    [SerializeField] private List<Material> _baloonCustomMaterials;

    [SerializeField] public float WindStrength = 0.5f;
    [SerializeField] public float MaxSwingAngle = 10f;

    private Quaternion[] _startingRotations;
    private Vector3 _windDirection;

    private Transform[] _transforms;
    private float[] _timeShifts;
    private MeshRenderer[] meshRenderers;

    private void Awake()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _transforms = new Transform[meshRenderers.Length];
        _timeShifts = new float[meshRenderers.Length];
        _startingRotations = new Quaternion[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            _transforms[i] = meshRenderers[i].transform;
            _timeShifts[i] = Random.Range(0.0f, 10.0f);
            _startingRotations[i] = _transforms[i].rotation;
        }

        // ���������� ��������� ����������� ����� (����� ����� ������ ��� �������)
        _windDirection = Random.insideUnitSphere.normalized;
    }

    private void Update()
    {
        for (int i = 0; i < _transforms.Length; i++)
        {
            ApplyWind(i);
        }
    }

    private void ApplyWind(int objIndex)
    {
        if (objIndex < 0 || objIndex >= _transforms.Length)
        {
            return;
        }

        if (_transforms[objIndex] == null)
        {
            return;
        }

        // ��������� ������� ��� ��������� �����
        float swingAngle = MaxSwingAngle * Mathf.Sin((Time.time + _timeShifts[objIndex]) * WindStrength);
        Quaternion swingRotation = Quaternion.AngleAxis(swingAngle, _windDirection);

        // ��������� ��������� � ��������� � ��������� �����������
        _transforms[objIndex].rotation = _startingRotations[objIndex] * swingRotation;
    }

    public void Init(StaticObject staticObject, CustomBehaviourNetworkObject customBehaviourNetworkObject)
    {
        if (meshRenderers.Length > 0)
        {
            if (staticObject.PlaceNumber >= 10)
            {
                foreach (MeshRenderer meshRenderer in meshRenderers)
                {
                    int materialsNumber = Random.Range(0, _baloonCustomMaterials.Count);
                    meshRenderer.material = _baloonCustomMaterials[materialsNumber];
                }
            }
        }
    }
}