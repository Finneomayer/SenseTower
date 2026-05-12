using SenseAI.Base;
using System.Collections;
using UnityEngine;

namespace SenseAI.Intefaces
{
    public interface IDeviceCommand
    {
        public void Do();
        public void Undo();
    }
}