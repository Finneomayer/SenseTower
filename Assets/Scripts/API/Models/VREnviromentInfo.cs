using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class VREnvironmentInfo
    {
        public string GetTokenEndpoint { get; set; }
        public string RegistrationPageEndpoint { get; set; }
        public string RegistrationEndpoint { get; set; }
        public string RegistrationAsGuestEndPoint { get; set; }
        public string MakeGuestResidentEndpoint { get; set; }
        public string ResetPasswordPageEndPoint { get; set; }
        public string RefreshTokenPageEndPoint { get; set; }
        public string SpacesEndpoint { get; set; }
        public string GetImagePageEndPoint { get; set; }
        public string GetMyPlacePageEndPoint { get; set; }
        public string GetMySpacePageEndPoint { get; set; }
        public string UpdateMyPlaceEndPoint { get; set; }
        public string SetAvatarEndPoint { get; set; }

        public string
            SetAvatarWatchEndPoint
        {
            get;
            set;
        } //https://dev.sensetower.io/accounts/api/v1/accounts/userinfo/setavatarwatch

        public string
            SetOculusAvatarWatchEndPoint
        {
            get;
            set;
        } //https://dev.sensetower.io/accounts/api/v1/accounts/userinfo/setoculusavatarwatch
        public string GetAllUsersEndPoint { get; set; }
        public string UsersInSpacesEndPoint { get; set; }
        public string GetSpacesUrlEndPoint { get; set; }
        public string GetSpacesUrlEndPointV2 { get; set; }
        public string GetTowerEventsUrlEndPoint { get; set; }
        public string GetTowerNewsUrlEndPoint { get; set; }
        public string RegistrationInSpacesEndPoint { get; set; }
        public string CheckUserAccessToSpacesEndPoint { get; set; }
        public string GetHallsEndPoint { get; set; }
        public string GetGalleryEndPoint { get; set; }
        public string GetRemoteContentLocationEndPoint { get; set; }
        public string GetRemoteSceneObjectLocationEndPoint { get; set; }
        public string ReplaceAllMyPlacePicturesUrl { get; set; }
        public string GetMyPlaceBySpaceUrl { get; set; }
        public string GetMyPlaceUrl { get; set; }
        public string UpdateDoorImageUrl { get; set; }
        public string GetTicketsEndPoint { get; set; }
        public string GetCinemasEndPoint { get; set; }
        public string GetScreenSharingEndPoint { get; set; }
        public string GetBroadcastingServiceEndPoint { get; set; }
        public string GetBroadcastingKey { get; set; }
        public string ReleaseVersion { get; set; }
        public int Bundle { get; set; }
        public string MetricaAppKey { get; set; }
        public string GetWalletEndPoint { get; set; }
        public string GetTransactionsEndPoint { get; set; }
        public string SpaceAccessPaymentsEndPoint { get; set; }
        public string SetPaymentSpaceAccessTypeEndPoint { get; set; }
        public string SetWhoInsideAccessTypeEndPoint { get; set; }
        public string GetTowerObjectsEndPoint { get; set; }
        public string GetShopsEndPoint { get; set; }
        public string FriendsEndPoint { get; set; }
        public string GetBuySellContractsEndPoint { get; set; }
        public string LeftSideHallAdvertisementBilboardEndPoint { get; set; }
        public string RightSideHallAdvertisementBilboardEndPoint { get; set; }
        public string GetSpaceStaticObjectsEndPoint { get; set; }
        public string GetSpaceObjectsNewEndPoint { get; set; }
        public string GetTowerObjectsClassEndPoint { get; set; }
        public string GetTowerObjectsRevisionEndPoint { get; set; }
        public string SendSpacePurchaseRequestEndPoint { get; set; }
        public string GetTipsEndPoint { get; set; }
        public string GetUserInfoEndPoint { get; set; }
        public string GetPaymentEndpoint { get; set; }
        public string GetMafiaEndPoint { get; set; }
        public string GetUserHoursEndPoint { get; set; }
        public string GetUserSellerStatus { get; set; }
        public string GetUserFullFledgedStatusEndPoint { get; set; }
        public string GetUserInitialBonusEndPoint { get; set; }
        public string GetDateWhenInitialBonusStarted { get; set; }
    }
}