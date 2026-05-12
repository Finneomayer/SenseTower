using SenseAI.Base;
using SenseAI.Intefaces;
using SenseAI.Robot.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace SenseAI.Robot
{

    public class RobotDevice : DeviceBase
    {
        private const float MinDistanceToCompleteMoving = 0.1f;
        private const float SqrMinDistanceToCompleteMoving = MinDistanceToCompleteMoving * MinDistanceToCompleteMoving;
        
        [Header("Main Robot Settings:")]
        private NavMeshAgent _agent;
        [SerializeField, Range(1, 10)] float _speed;


        [Header("Robot Navigtion:")]
        Vector3 _destination;
        [SerializeField] private bool _isBusy;
        [SerializeField] private bool _onWayToTarget;
        [SerializeField] private bool _onWayToBase;
        [SerializeField] private MVPNavigation _mvpNavigation;

        [Header("Robot output info:")]
        string _outputInfo;
        private Vector3 _prevPos;
        [Header("Events:")]
        public UnityEvent OnOutPutInfoSetted = new UnityEvent();
        [Header("Line settings:")]
        private RobotLineController _robotLine;

        private Coroutine _delayedReturnToBaseRoutine;
        private Coroutine _checkReachedDestinationRoutine;

        public Action<Vector3[]> PathCalculated;

        public Vector3 Destination
        {
            get
            {
                return _destination;
            }
            set
            {
                _destination = value;
                SetDestination(_destination);
            }
        }

        public string OutputInfo
        {
            get => _outputInfo;
            set
            {
                _outputInfo = value;
                OnOutPutInfoSetted.Invoke();
            }
        }

        public string DeviceOwner { get; internal set; }
        public bool IsBusy 
        { 
            get => _isBusy; 
            set {
                _isBusy = value;
                BusyStateChanged?.Invoke();
            }
        }

        public bool OnWayToTarget { get => _onWayToTarget; set => _onWayToTarget = value; }
        public bool OnWayToBase { get => _onWayToBase; set => _onWayToBase = value; }

        public event Action BusyStateChanged;

        private void Awake()
        {
            Cashing();
            Settings();
        }

        private void OnEnable()
        {
            _robotLine.PathCalculated += OnRobotLinePathCalculated;
        }

        private void OnDisable()
        {
            _robotLine.PathCalculated -= OnRobotLinePathCalculated;
        }

        public void DrawPath(Vector3[] path)
        {
            _robotLine.SetPath(path);
        }

        public void ClearPath()
        {
            _robotLine.ClearPath();
        }

        public void SetDestination(Vector3 destination)
        {
            if (_agent.isOnNavMesh)
            {
                IsBusy = true;
                OnWayToTarget = true;
                OnWayToBase = false;
                _agent.SetDestination(destination);
                _robotLine.SetPathByAgent(_agent);

                if (_checkReachedDestinationRoutine != null)
                {
                    StopCoroutine(_checkReachedDestinationRoutine);
                }
                _checkReachedDestinationRoutine = StartCoroutine(CheckPathIsComplete(destination));
            }
        }

        public void ReturnToBase()
        {
            _mvpNavigation.GoToBaseImmediatlley();
        }

        public void CancelMoveToDestination()
        {
            _robotLine.ClearPath();

            IsBusy = false;
            OnWayToTarget = false;
            if (_checkReachedDestinationRoutine != null)
            {
                StopCoroutine(_checkReachedDestinationRoutine);
                _checkReachedDestinationRoutine = null;
            }
            if (_delayedReturnToBaseRoutine != null)
            {
                StopCoroutine(_delayedReturnToBaseRoutine);
                _delayedReturnToBaseRoutine = null;
            }

        }

        private void FinishMoveToDestination()
        {
            _robotLine.ClearPath();

            IsBusy = false;
            OnWayToTarget = false;

            if (_delayedReturnToBaseRoutine != null)
            {
                StopCoroutine(_delayedReturnToBaseRoutine);
            }
            _delayedReturnToBaseRoutine = StartCoroutine(ReturnToBaseAfterDalayRoutine());

            _mvpNavigation.FinishMoveToDestination();
        }

        private IEnumerator ReturnToBaseAfterDalayRoutine()
        {
            yield return new WaitForSeconds(15);
            ReturnToBase();

            _delayedReturnToBaseRoutine = null;
        }

        private void Cashing()
        {
            _agent = GetComponent<NavMeshAgent>();
            _robotLine = GetComponentInChildren<Robot.UI.RobotLineController>();
            _mvpNavigation = GetComponent<MVPNavigation>();
        }
        private void Settings()
        {
            DeviceOwner = "Meta Sense";
            _agent.speed = _speed;
            IsBusy = false;
            OnWayToTarget = false;
        }

        private void OnRobotLinePathCalculated(Vector3[] path)
        {
            PathCalculated?.Invoke(path);
        }

        public override void Do()
        {
            _prevPos = transform.position;
            Destination = transform.forward * 5;
        }

        public override void Undo()
        {
            Destination = _prevPos;
        }

        #region CoRoutines

        IEnumerator CheckPathIsComplete(Vector3 destination)
        {
            bool isDestinationReached = false;
            while (_agent.pathPending)
            {
                yield return new WaitForSeconds(0.1f);
                
                isDestinationReached = IsDestinationReached(destination);

                if (isDestinationReached)
                {
                    break;
                }
            }

            if (!isDestinationReached)
            {
                while (_agent.remainingDistance > MinDistanceToCompleteMoving)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }

            FinishMoveToDestination();
            _checkReachedDestinationRoutine = null;
        }

        private bool IsDestinationReached(Vector3 destination)
        {
            return (_agent.transform.position - destination).sqrMagnitude < SqrMinDistanceToCompleteMoving;
        }


        #endregion
    }
}