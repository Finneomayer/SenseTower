using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.Interaction.Toolkit;

namespace SenseAI.Robot
{
    public class RobotInteractorReleationController : MonoBehaviour
    {
        private const float SPEED = 3;
        private const int WAINTING_SEC = 15; // TBD
        private const int MAX_DISTANCE = 3;
        [SerializeField, Range(0, 100)] float _distance;
        [SerializeField] private NavMeshAgent _agent;
        private Vector3 _currentInteractorPosition;
        private RobotDevice _robot;
        Vector3 _base;
        [SerializeField, Range(0, WAINTING_SEC)] private int _stopWatch;

        private void Awake() => Cashing();
        private void Cashing()
        {
            _base = transform.position;
            _distance = 0;
            _stopWatch = 0;
            if (_agent == null)
            {
                _agent = GetComponent<NavMeshAgent>();
            }
            if (_robot == null)
            {
                _robot = GetComponent<RobotDevice>();
            }
        }

        public void RefreshInteractorPosition(Vector3 newPosition)
        {
            _currentInteractorPosition = newPosition;
        }

        public void StartEscortToTarget(Vector3 interactorPosition)
        {
            StopAllCoroutines();

            RefreshInteractorPosition(interactorPosition);
            StartCoroutine(LiteUpdate());
            StartCoroutine(StopWatch());
        }

        public void StopEscortToTarget()
        {
            StopAllCoroutines();
            _agent.speed = SPEED;
        }

        private void OnDisable() => StopAllCoroutines();

        IEnumerator LiteUpdate()
        {
            while (gameObject.activeInHierarchy)
            {
                yield return new WaitForSeconds(0.5f);
                _distance = Vector3.Distance(_currentInteractorPosition, transform.position); 
                var followingCondition = _distance > MAX_DISTANCE && _robot.IsBusy; 
                _agent.speed = followingCondition ? 0 : SPEED;
            }
        }

        private IEnumerator StopWatch()
        {
            _stopWatch = 0;
             while(gameObject.activeInHierarchy)
            {
                yield return new WaitForSeconds(1);
                _stopWatch = _distance >= 3 ? ++_stopWatch : 0;
                if(_stopWatch >= WAINTING_SEC && (_robot.IsBusy || _robot.OnWayToBase))
                {
                    _stopWatch = 0;
                    _robot.CancelMoveToDestination();
                    _robot.ReturnToBase();
                }
            }
        }
    }
}