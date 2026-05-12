using SenseAI.Robot;
using SenseAI.Robot.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace SenseAI.Robot
{
    public class MVPNavigation : MonoBehaviour
    {
        private Vector3 _startPosition, _targetPosition;
        [SerializeField] private RobotDevice _robot;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] Transform _target;
        [SerializeField] InfoPanelController _infoPanel;

        public event Action GoToBaseStarted;
        public event Action GoToOfficeFinished;

        void Awake()
        {
            _startPosition = transform.position;
            _targetPosition = _target.position;
        }
        
        public void GoToBaseImmediatlley()
        {            
            _agent.SetDestination(_startPosition);
            _infoPanel.HideEveryrhing();

            GoToBaseStarted?.Invoke();
        }

        public void FinishMoveToDestination()
        {
            GoToOfficeFinished?.Invoke();
        }

        public void GoToOffice()
        {
            _robot.OnWayToBase = false;
            _robot.Destination = _targetPosition;
        }

        public void InterupRobotOnWay()
        {
            if (!_robot.IsBusy)
                _agent.SetDestination(transform.position);
        }
    }
}