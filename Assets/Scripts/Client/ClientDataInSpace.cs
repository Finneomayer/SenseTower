using Assets.Scripts.Data;
using UnityEngine;

namespace Client
{
    public class ClientDataInSpace
    {
        private const string LastSpaceIDKey = "LastSpaceID";
        private const string LastSpaceNameKey = "LastSpaceName";
        private const string LastSpacePositionKey = "LastSpacePosition";
        private const string LastSPaceRotationKey = "LastSPaceRotation";
        private const string LastOccupiedPlaceNetworkObjectIdKey = "LastOccupiedPlaceNetworkObjectId";
        
        public Vector3 LastSpacePosition
        {
            get
            {
                if (!PlayerPrefs.HasKey(LastSpacePositionKey))
                {
                    PlayerPrefs.SetString(LastSpacePositionKey, null);
                }
                
                return PlayerPrefs.GetString(LastSpacePositionKey).StringToVector3();
            }
            set => PlayerPrefs.SetString(LastSpacePositionKey,value.Vector3ToCustomString() );
        }

        public Vector3 LastSpaceRotation
        {
            get
            {
                if (!PlayerPrefs.HasKey(LastSPaceRotationKey))
                {
                    PlayerPrefs.SetString(LastSPaceRotationKey, null);
                }
                return PlayerPrefs.GetString(LastSPaceRotationKey).StringToVector3();
            }
            set => PlayerPrefs.SetString(LastSPaceRotationKey,  value.Vector3ToCustomString());
        }

        public string LastSpaceID
        {
            get
            {
                if (!PlayerPrefs.HasKey(LastSpaceIDKey))
                {
                    PlayerPrefs.SetString(LastSpaceIDKey, null);
                }
                return PlayerPrefs.GetString(LastSpaceIDKey);
            }
            set => PlayerPrefs.SetString(LastSpaceIDKey,  value);
        }
        
        public string LastSpaceName
        {
            get
            {
                if (!PlayerPrefs.HasKey(LastSpaceNameKey))
                {
                    PlayerPrefs.SetString(LastSpaceNameKey, null);
                }
                return PlayerPrefs.GetString(LastSpaceNameKey);
            }
            set => PlayerPrefs.SetString(LastSpaceNameKey,  value);
        }

        public string OccupiedPlaceNetworkObjectId
        {
            get
            {
                if (!PlayerPrefs.HasKey(LastOccupiedPlaceNetworkObjectIdKey))
                {
                    PlayerPrefs.SetString(LastOccupiedPlaceNetworkObjectIdKey, null);
                }
                return PlayerPrefs.GetString(LastOccupiedPlaceNetworkObjectIdKey);
            }
            set => PlayerPrefs.SetString(LastOccupiedPlaceNetworkObjectIdKey, value);
        }

        public void Clear()
        {
            LastSpaceName = string.Empty;
            LastSpaceID = string.Empty;
            LastSpaceRotation = Vector3.zero;
            LastSpacePosition = Vector3.zero;
            OccupiedPlaceNetworkObjectId = string.Empty;
        }
    }
}