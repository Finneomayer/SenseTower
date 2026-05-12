using System;
using API.Models;
using Assets.Scripts.Space;
using UnityEngine;

namespace Assets.Scripts.Client
{
    public sealed class ClientData : IClientData
    {
        private static SpaceDoorData _lastSpaceDoorData; //needs to player know where to respawn in Hall

        private const string AccessTokenKey = "AccessToken";
        private const string TypeOwnerKey = "OwnerType";
        private const string UserIdKey = "UserId";
        private const string UserNameKey = "UserName";
        private const string IsGuestKey = "IsGuest";
        private const string RefreshTokenKey = "RefreshToken";
        private const string AssemblyTypeKey = "AssemblyType";
        private const string AuthTokenUnixTimeKey = "AuthTokenUnixTime";
        private const string ExpTokenUnixTimeKey = "ExpTokenUnixTime";
        private const string OwnedSpacesNumberKey = "OwnedSpacesNumber";
        private const string CreatedOnKey = "CreatedOnKey";

        public event Action DeleteAllAuthData;

        public SpaceDoorData LastSpaceDoorData => _lastSpaceDoorData;

        public bool IsRefreshing { get; set; }

        public OwnerType TypeOwner
        {
            get
            {
                if (!PlayerPrefs.HasKey(TypeOwnerKey))
                {
                    PlayerPrefs.SetInt(TypeOwnerKey, (int) OwnerType.Undefined);
                }

                return (OwnerType) PlayerPrefs.GetInt(TypeOwnerKey);
            }
            set => PlayerPrefs.SetInt(TypeOwnerKey, (int) value);
        }

        public Guid? UserId
        {
            get
            {
                if (!PlayerPrefs.HasKey(UserIdKey))
                {
                    PlayerPrefs.SetString(UserIdKey, null);
                }

                var guidString = PlayerPrefs.GetString(UserIdKey);
                if (string.IsNullOrEmpty(guidString))
                    return null;
                return Guid.Parse(guidString);
            }
            set => PlayerPrefs.SetString(UserIdKey, value.ToString());
        }

        public string UserName
        {
            get
            {
                if (!PlayerPrefs.HasKey(UserNameKey))
                {
                    PlayerPrefs.SetString(UserNameKey, null);
                }

                return PlayerPrefs.GetString(UserNameKey);
            }
            set => PlayerPrefs.SetString(UserNameKey, value);
        }

        public bool IsGuest
        {
            get
            {
                if (!PlayerPrefs.HasKey(IsGuestKey))
                {
                    PlayerPrefs.SetString(IsGuestKey, false.ToString());
                }

                string isGuestInString = PlayerPrefs.GetString(IsGuestKey);
                if (bool.TryParse(isGuestInString, out bool IsGuest))
                    return IsGuest;
                
                return false;
            }
            set => PlayerPrefs.SetString(IsGuestKey, value.ToString());
        }

        public string AccessToken
        {
            get
            {
                if (!PlayerPrefs.HasKey(AccessTokenKey))
                {
                    PlayerPrefs.SetString(AccessTokenKey, null);
                }

                return PlayerPrefs.GetString(AccessTokenKey);
            }
            set => PlayerPrefs.SetString(AccessTokenKey, value);
        }

        public string RefreshToken
        {
            get
            {
                if (!PlayerPrefs.HasKey(RefreshTokenKey))
                {
                    PlayerPrefs.SetString(RefreshTokenKey, null);
                }

                return PlayerPrefs.GetString(RefreshTokenKey);
            }
            set => PlayerPrefs.SetString(RefreshTokenKey, value);
        }

        public string AuthTokenUnixTime
        {
            get
            {
                if (!PlayerPrefs.HasKey(AuthTokenUnixTimeKey))
                {
                    PlayerPrefs.SetString(AuthTokenUnixTimeKey, null);
                }

                return PlayerPrefs.GetString(AuthTokenUnixTimeKey);
            }
            set => PlayerPrefs.SetString(AuthTokenUnixTimeKey, value);
        }

        public string ExpTokenUnixTime
        {
            get
            {
                if (!PlayerPrefs.HasKey(ExpTokenUnixTimeKey))
                {
                    PlayerPrefs.SetString(ExpTokenUnixTimeKey, null);
                }

                return PlayerPrefs.GetString(ExpTokenUnixTimeKey);
            }
            set => PlayerPrefs.SetString(ExpTokenUnixTimeKey, value);
        }

        public int OwnedSpacesNumber
        {
            get
            {
                if (!PlayerPrefs.HasKey(OwnedSpacesNumberKey))
                {
                    PlayerPrefs.SetInt(OwnedSpacesNumberKey, 0);
                }

                return PlayerPrefs.GetInt(OwnedSpacesNumberKey);
            }
            set => PlayerPrefs.SetInt(OwnedSpacesNumberKey, value);
        }

        public string AssemblyType
        {
            get
            {
                if (!PlayerPrefs.HasKey(AssemblyTypeKey))
                {
                    PlayerPrefs.SetString(AssemblyTypeKey, null);
                }

                return PlayerPrefs.GetString(AssemblyTypeKey);
            }
            set => PlayerPrefs.SetString(AssemblyTypeKey, value);
        }

        public DateTimeOffset CreatedOn
        {
            get
            {
                if (!PlayerPrefs.HasKey(CreatedOnKey))
                {
                    PlayerPrefs.SetString(CreatedOnKey, null);
                }

                var dateDefault = DateTimeOffset.MinValue;

                if (DateTimeOffset.TryParse(PlayerPrefs.GetString(CreatedOnKey), out DateTimeOffset date))
                    dateDefault = date;

                return dateDefault;
            }
            set => PlayerPrefs.SetString(CreatedOnKey, value.ToString());
        }

        public void DeleteAllData()
        {
            if (PlayerPrefs.HasKey(RefreshTokenKey))
                PlayerPrefs.DeleteKey(RefreshTokenKey);

            if (PlayerPrefs.HasKey(AccessTokenKey))
                PlayerPrefs.DeleteKey(AccessTokenKey);

            if (PlayerPrefs.HasKey(UserNameKey))
                PlayerPrefs.DeleteKey(UserNameKey);

            if (PlayerPrefs.HasKey(UserIdKey))
                PlayerPrefs.DeleteKey(UserIdKey);

            if (PlayerPrefs.HasKey(TypeOwnerKey))
                PlayerPrefs.DeleteKey(TypeOwnerKey);

            if (PlayerPrefs.HasKey(AuthTokenUnixTimeKey))
                PlayerPrefs.DeleteKey(AuthTokenUnixTimeKey);

            if (PlayerPrefs.HasKey(ExpTokenUnixTimeKey))
                PlayerPrefs.DeleteKey(ExpTokenUnixTimeKey);

            if (PlayerPrefs.HasKey(OwnedSpacesNumberKey))
                PlayerPrefs.DeleteKey(OwnedSpacesNumberKey);

            if (PlayerPrefs.HasKey(IsGuestKey))
                PlayerPrefs.DeleteKey(IsGuestKey);

            if (PlayerPrefs.HasKey(CreatedOnKey))
                PlayerPrefs.DeleteKey(CreatedOnKey);

            DeleteAllAuthData?.Invoke();
        }

        public void SetLastSpaceDoorData(SpaceDoorData doorData)
        {
            _lastSpaceDoorData = doorData;
        }
    }
}