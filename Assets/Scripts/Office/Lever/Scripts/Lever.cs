using Assets.Mechanics.Network.Scripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HingeJoint))]
public class Lever : NetworkBehaviour
{

    [SerializeField]
    private float _leverOnAngle = -45;
    [SerializeField]
    private float _leverOffAngle = 45;

    private bool _leverIsOn = true;

    public UnityEvent<bool> OnSwitchLever;
    [SerializeField]
    private float _scale = 5;

    private HingeJoint _leverHingeJoint;

    private InterectObjectGrabbable _grabbable;

    private NetworkVariable<bool> _wasGrabbed;
    private NetworkVariable<float> _targetSpring;
    private NetworkVariable<ulong> _handID;


    private Vector3 _startingEuler;
    [SerializeField]
    private float _timeConst = 1f;
    private float _timeStart;

    private void Awake()
    {
        _wasGrabbed = new NetworkVariable<bool>();
        _targetSpring = new NetworkVariable<float>();
        _handID = new NetworkVariable<ulong>();
    }

    private void Start()
    {
        _leverHingeJoint = GetComponent<HingeJoint>();

        JointLimits limits = _leverHingeJoint.limits;
        limits.max = Mathf.Max(_leverOnAngle, _leverOffAngle);
        limits.min = Mathf.Min(_leverOnAngle, _leverOffAngle);
        _leverHingeJoint.limits = limits;
        _leverHingeJoint.useLimits = true;

        InterectObjectGrabbable[] grabbables = (InterectObjectGrabbable[])AllComponentsOfType<InterectObjectGrabbable>(gameObject);

        _grabbable = grabbables[0];
        _startingEuler = this.transform.localEulerAngles;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            _leverHingeJoint = GetComponent<HingeJoint>();
            _wasGrabbed.Value = false;
            _targetSpring.Value = _leverOnAngle;
            SetSpringJoint(_leverOnAngle);
        }
        else
        {
            transform.localScale = Vector3.one * _scale;
        }
    }

    private void Update()
    {
        if (!IsSpawned) return;

        if (IsServer)
        {
            if (_wasGrabbed.Value && Time.time > _timeConst + _timeStart)
            {
                _wasGrabbed.Value = false;
                _handID.Value = 0;
            }

            ChechSwitchLever();
            // UpdateHingeJointServerRpc();
            UpdateHingeJoint();
        }
        else
        {
            var id = NetworkManager.LocalClientId;
            if (_grabbable.inHand)
            {
                ActiveHandServerRpc(id);
            }

            if (_handID.Value == id && _grabbable.inHand)
            {
                var angle = transform.localRotation.eulerAngles.z;
                if (angle >= 360 + _leverOnAngle)
                    angle -= 360;
                SetAngleServerRpc(angle);
                _leverHingeJoint.useSpring = false;
            }

            if (!_wasGrabbed.Value)
                _leverHingeJoint.useSpring = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetAngleServerRpc(float angle)
    {
        if (angle >= 360 + _leverOnAngle)
            angle -= 360;

        _targetSpring.Value = angle;
    }

    private void ChechSwitchLever()
    {
        float offDistance = Quaternion.Angle(this.transform.localRotation, OffHingeAngle());
        float onDistance = Quaternion.Angle(this.transform.localRotation, OnHingeAngle());

        bool shouldBeOn = (Mathf.Abs(onDistance) < Mathf.Abs(offDistance));
        if (shouldBeOn != _leverIsOn)
        {
            _leverIsOn = !_leverIsOn;
            OnSwitchLever?.Invoke(_leverIsOn);
        }
    }
    //ToDo эксперемент. удачно. надо кудато записать. не забыть. 
    private Component[] AllComponentsOfType<T>(GameObject parent) where T : Component
    {
        T parentComponent = null;
        if (parent.GetComponent<T>())
        {
            parentComponent = parent.GetComponent<T>();
        }

        T[] componets = parent.GetComponentsInChildren<T>();

        if (parentComponent != null)
        {
            T[] returnComponents = new T[componets.Length + 1];
            for (int i = 0; i < componets.Length; i++)
            {
                returnComponents[i] = componets[i];
            }
            returnComponents[componets.Length] = parentComponent;

            return returnComponents;
        }

        return componets;
    }

    private void UpdateHingeJoint()
    {
        _leverHingeJoint.useSpring = true;

        if (!_wasGrabbed.Value)
        {
            _targetSpring.Value = (_leverIsOn) ? _leverOnAngle : _leverOffAngle;
        }

        SetSpringJoint(_targetSpring.Value);
        SetTransformClientRpc(_targetSpring.Value);

    }

    [ServerRpc(RequireOwnership = false)]
    private void ActiveHandServerRpc(ulong id)
    {
        _handID.Value = id;
        _timeStart = Time.time;
        _wasGrabbed.Value = true;
    }



    [ClientRpc]
    private void SetTransformClientRpc(float target)
    {
        if (_handID.Value != NetworkManager.Singleton.LocalClientId)
        {
            SetSpringJoint(target);
        }
    }

    private void SetSpringJoint(float target)
    {
        JointSpring spring = _leverHingeJoint.spring;
        spring.targetPosition = target;
        _leverHingeJoint.spring = spring;
    }


    private Quaternion OnHingeAngle()
    {
        return Quaternion.Euler(this._leverHingeJoint.axis * _leverOnAngle + _startingEuler);
    }

    private Quaternion OffHingeAngle()
    {
        return Quaternion.Euler(this._leverHingeJoint.axis * _leverOffAngle + _startingEuler);
    }
}
