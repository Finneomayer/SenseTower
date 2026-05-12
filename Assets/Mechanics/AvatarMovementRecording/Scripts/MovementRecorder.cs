using Oculus.Avatar2;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Mechanics.AvatarMovementRecording.Scripts
{
    public class MovementRecorder
    {
        private byte[][] _frame;
        private List<AvatarFrame[]> _savedData;
        private List<AvatarFrame[]> _openedData;
        private int _currentFrameNumber;

        public void Init()
        {
            _savedData = new List<AvatarFrame[]>();
        }

        public void AddFrame(AvatarFrame[] frame)
        {
            _savedData.Add(frame);
        }

        public AvatarFrame[] GetNextFrame()
        {
            if (_currentFrameNumber >= _openedData.Count) return null;

            var result = _openedData[_currentFrameNumber];
            _currentFrameNumber++;
            return result;
        }

        public void ClearCounter()
        {
            _currentFrameNumber = 0;
        }

        public List<AvatarFrame[]> GetRecord()
        {
            return _savedData;
        }

        public void SetRecord(List<AvatarFrame[]> record)
        {
            _openedData = record;
        }
    }
}
