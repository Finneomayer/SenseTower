using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace SenseAI.Robot
{

    public class RobotTransform : MonoBehaviour
    {
        public void LookAtIterator(SelectEnterEventArgs args)
        {
            if (args.interactor == null)
                return;
            LookAtByYAxis(args);
        }

        public void LookAtPosition(Vector3 position)
        {
            position.y = transform.position.y;
            transform.LookAt(position);
            //transform.LookAt(position);
            //float curentYAngle = transform.eulerAngles.y + 180;
            //transform.eulerAngles = new Vector3(0, curentYAngle, 0);
        }

        public void LookAtObject(Transform objectTransform)
        {
            transform.LookAt(objectTransform.position);
            float curentYAngle = transform.eulerAngles.y + 180;
            transform.eulerAngles = new Vector3(0, curentYAngle, 0);
        }

        private void LookAtByYAxis(SelectEnterEventArgs args)
        {
            Transform attention = args.interactor.transform;
            transform.LookAt(attention.position);
            float curentYAngle = transform.eulerAngles.y + 180;
            transform.eulerAngles = new Vector3(0, curentYAngle, 0);
        }

    }
}