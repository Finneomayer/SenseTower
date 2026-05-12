using SenseAI.Intefaces;
using System.Collections;
using UnityEngine;

namespace SenseAI.Base
{
    public abstract class DeviceCommand : IDeviceCommand
    {
        public abstract void DeviceInnit(DeviceBase device);
        public abstract void Do();

        public abstract void Undo();

        public abstract void Stop();
    }
}