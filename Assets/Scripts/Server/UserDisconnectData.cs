using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Assets.Scripts.Server
{
    public class UserDisconnectData
    {
        public string DisconnectMessage { get; set; } = "";
        public bool CanBeInSenseTower { get; set; }

        public static UserDisconnectData Deserialize(string serializedString)
        {
            UserDisconnectData deserialized;
            try
            {
                deserialized = JsonConvert.DeserializeObject<UserDisconnectData>(serializedString);
            }
            catch (Exception e)
            {
                Debug.Log($"<color=red>Error deserialize {typeof(UserDisconnectData)}: </color>" + e);
                deserialized = new UserDisconnectData();
            }

            return deserialized;
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}