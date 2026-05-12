using System.Collections;
using UnityEngine;

namespace SenseAI.Robot.UI
{
    public class RoomButtonController : MonoBehaviour 
    {
        private RobotDevice _robot;
        private Transform _target;
        InfoPanelController _inpoPanel;
        private void OnEnable()
        {
            _robot = GetComponentInParent<RobotDevice>();
            _inpoPanel = GetComponentInParent<InfoPanelController>();
        }
        public void ApplyTarget(Transform target)
        { 
            _target = target;
        }
        public void SendCommandToRobot()
        {
            StartCoroutine(DelayedSendCommand());
        }

       IEnumerator DelayedSendCommand()
        {
            //_robot.OutputInfo = "Следуй за мной!";
             yield return new WaitForSeconds(3f);
            if (_robot != null)
            {
                _robot.Destination = _target.position;
            }
            _inpoPanel.OnHideMainMenu.Invoke();
        }
    }
}