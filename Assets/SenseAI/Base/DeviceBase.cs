using SenseAI.Intefaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SenseAI.Base
{
    public abstract class DeviceBase : MonoBehaviour
    {
        public abstract void Do();
        public abstract void Undo();

        public void ApplyCommand(IDeviceCommand commad)
        {
            commad.Do(); 
        }

    }
}