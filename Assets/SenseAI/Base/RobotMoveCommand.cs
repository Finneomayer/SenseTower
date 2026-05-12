using SenseAI.Intefaces;

namespace SenseAI.Base
{
    public class RobotMoveCommand : DeviceCommand, IDeviceCommand
    {
        DeviceBase _device;

        public override void DeviceInnit(DeviceBase device)
        {
            _device = device;
        }

        public override void Do()
        {
            if (_device != null)
            {
                _device.Do();
            }
        }

        public override void Stop()
        {
            throw new System.NotImplementedException();
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}