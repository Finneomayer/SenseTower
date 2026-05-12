using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SenseAI.Base;

namespace SenseAI.Robot
{
    public class RobotAdapter : MonoBehaviour
    {
        DeviceBase device; 
        private void Start()
        {
            device = GetComponent<DeviceBase>();
        }
        private void Update()
        { 
        } 
    }
}