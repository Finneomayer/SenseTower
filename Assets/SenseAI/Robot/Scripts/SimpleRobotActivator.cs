using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SenseAI.Robot.Scripts
{
    public class SimpleRobotActivator : MonoBehaviour
    {
        [SerializeField] private RobotTransform RobotTransform;
        [SerializeField] private XRBaseInteractable XRInteractable;

        private void OnEnable()
        {
            XRInteractable.firstSelectEntered.AddListener(OnXRIntaractableSelectEnter);
        }

        private void OnDisable()
        {
            XRInteractable.firstSelectEntered.RemoveListener(OnXRIntaractableSelectEnter);
        }

        private void OnXRIntaractableSelectEnter(SelectEnterEventArgs args)
        {
            RobotTransform.LookAtPosition(args.interactorObject.transform.position);
        }
    }
}