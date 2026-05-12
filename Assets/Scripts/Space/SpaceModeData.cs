using Assets.Scripts.Client;
using Data;
using UnityEngine;

namespace Assets.Scripts.Space
{
    public class SpaceModeData : ISpaceModeData
    {
        private static Enumenators.SpaceModeType _spaceModeType = Enumenators.SpaceModeType.Normal;
        public Enumenators.SpaceModeType SpaceModeType
        {
            get => _spaceModeType;
            set => _spaceModeType = value;
        }
    }
}