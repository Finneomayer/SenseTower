using System;
using UnityEngine;

namespace Assets.Mechanics.MetaAvatars.Scripts
{
    public static class AvatarSessionData
    {
        public const int DefaultAvatarId = 9999;
        public const int DefaultAvatarAssetId = 9999;
        private const string AvatarAssetIdKey = "AvatarAssetId";

        public static bool IsUserCustomAvatarLoaded { get; private set; }
        public static ulong? UserId { get; private set; }

        public static event Action AvatarAssetIdChanged;

        public static int? AvatarAssetId
        {
            get
            {
                if (!PlayerPrefs.HasKey(AvatarAssetIdKey))
                {
                    return null;
                }
                return PlayerPrefs.GetInt(AvatarAssetIdKey);
            }
        }

        public static void SetAvatarAssetId(int assetId)
        {
            int? currentAssetId = AvatarAssetId;
            if (currentAssetId.HasValue && currentAssetId.Value == assetId)
            {
                return;
            }
            PlayerPrefs.SetInt(AvatarAssetIdKey, assetId);
            AvatarAssetIdChanged?.Invoke();
        }

        public static void SetUserId(ulong userId)
        {
            UserId = userId;
        }

        public static void SetCustomAvatarLoaded(bool loaded)
        {
            IsUserCustomAvatarLoaded = loaded;
        }
    }
}
