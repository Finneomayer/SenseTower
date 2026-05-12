using System;

namespace Assets.Scripts.Client
{
    public interface IClientData
    {
        Guid? UserId { get; set; }
        string UserName { get; set; }
        bool IsGuest { get; set; }

        OwnerType TypeOwner { get; set; }
        string AccessToken { get; set; }
        string RefreshToken { get; set; }
        public SpaceDoorData LastSpaceDoorData { get; }
        string AssemblyType { get; set; }
        bool IsRefreshing { get; set; }
        string AuthTokenUnixTime { get; set; }
        string ExpTokenUnixTime { get; set; }
        int OwnedSpacesNumber { get; set; }
        DateTimeOffset CreatedOn { get; set; } //user registration data

        void SetLastSpaceDoorData(SpaceDoorData doorData);
        void DeleteAllData();

        public event Action DeleteAllAuthData;
    }
}