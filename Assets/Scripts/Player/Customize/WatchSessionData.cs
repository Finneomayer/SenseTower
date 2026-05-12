using Assets.Mechanics.MetaAvatars.Scripts;
using System;
using UnityEngine;

public static class WatchSessionData 
{
    private const string WatchPlayerAssetId = "WatchPlayerAssetId";
    private const string WatchPlayerAssetIdOculus = "WatchPlayerAssetIdOculus";
    private static readonly int[] IgnoreAvatarId = new int[] { 0, 1, 5, 10, 11, 12, 16, 18, 20, 22, 25, 27, 28, 29 };

    public static event Action WatchIdChanged;
    public static bool HasIgnoreAvatar => IgnoreAvatar();

    public static bool IsCustomMetaAvatar =>
            (AvatarSessionData.UserId != null && AvatarSessionData.UserId != 0);

    public static int WatchPlayerId
    {
        get
        {
            if (!PlayerPrefs.HasKey(WatchPlayerAssetId))
            {
                return 0;
            }

            //if (AvatarSessionData.UserId != 0)
            //{
            //    Debug.LogWarning("Meta avatar");
            //    return 0;
            //}

            return !HasIgnoreAvatar ? PlayerPrefs.GetInt(WatchPlayerAssetId) : 0;
        }
        set
        {
            if (value >= 0) 
                PlayerPrefs.SetInt(WatchPlayerAssetId, value);

            WatchIdChanged?.Invoke();
            Debug.LogWarning($"Set watch id to {value}");
        }
    }

    public static int WatchPlayerIdOculus 
    {
        get
        {
            if (!PlayerPrefs.HasKey(WatchPlayerAssetIdOculus))
            {
                return 0;
            }

            return PlayerPrefs.GetInt(WatchPlayerAssetIdOculus);
        }
        set
        {
            if (value >= 0)
                PlayerPrefs.SetInt(WatchPlayerAssetIdOculus, value);

            WatchIdChanged?.Invoke();
            Debug.LogWarning($"Set watch oculus id to {value}");
        }
    }

    private static bool IgnoreAvatar()
    {
        var avatarId = AvatarSessionData.AvatarAssetId.Value;
        var ignore = Array.IndexOf(IgnoreAvatarId, avatarId) != -1;
        //return ignore;
        return false;
    }
}
