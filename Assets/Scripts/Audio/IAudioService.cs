using System;
using System.Collections.Generic;

namespace Assets.Scripts.Audio
{
    public interface IAudioService
    {
        Dictionary<ulong,string> MutedUsersID { get; }
        public void MuteUser(string userGuId);
        public void MuteUser(ulong userId);
        public void MuteUserForUserServer(string userGuid, string userGuidToMute);
        public void UnmuteUser(string userGuId);
        public void UnmuteUser(ulong userGuId);
        public void UnmuteUserForUserServer(string userGuid, string userGuidToUnmute);

        event Action<string> UserSoundEnabled;
        event Action<string> UserSoundDisabled;
    }
}