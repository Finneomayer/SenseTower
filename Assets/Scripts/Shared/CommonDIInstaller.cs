using Assets.Mechanics.AvatarMovementRecording.Scripts;
using Assets.Mechanics.FriendsList;
using Assets.Mechanics.InAppPurchase;
using Assets.Mechanics.Mafia;
using Assets.Mechanics.MetaAvatars.Scripts;
using Assets.Mechanics.MyPlaces;
using Assets.Mechanics.Tips;
using Assets.Scripts.API;
using Assets.Scripts.API.RegistrationService;
using Assets.Scripts.Cinema;
using Assets.Scripts.Client;
using Assets.Scripts.Event;
using Assets.Scripts.Gallery;
using Assets.Scripts.Hall;
using Assets.Scripts.News;
using Assets.Scripts.Server;
using Assets.Scripts.Space;
using Assets.Scripts.TowerObjects;
using Assets.Scripts.TowerObjectsClass;
using Assets.Scripts.TowerObjectsRevision;
using Assets.Scripts.TowerObjectsRevision.Interfaces;
using Assets.Scripts.Trading;
using Assets.Scripts.Transactions;
using Mechanics.InAppPurchase.Interfaces;
using Mechanics.SendPurchaseSpaceRequest;
using Mechanics.UserWallet.Service;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Shared
{
    public class CommonDIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IApiService>().To<APIService>().AsSingle();
            Container.Bind<IClientData>().To<ClientData>().AsSingle();
            Container.Bind<ISpaceModeData>().To<SpaceModeData>().AsSingle();
            Container.Bind<ITipsService>().To<TipsService>().AsSingle();
            Container.Bind<IServerApiService>().To<ServerApiService>().AsSingle();
            Container.Bind<IServerApiData>().To<ServerApiData>().AsSingle();
            Container.Bind<ISpaceService>().To<SpaceService>().AsSingle();
            Container.Bind<ISpaceManager>().To<SpaceManager>().AsSingle();
            Container.Bind<ITowerObjectsService>().To<TowerObjectsService>().AsSingle();
            Container.Bind<IRegistrationInSpacesService>().To<RegistrationInSpacesService>().AsTransient();
            Container.Bind<IHallsService>().To<HallsService>().AsTransient();
            Container.Bind<IGalleryService>().To<GalleryService>().AsTransient();
            Container.Bind<IMyImageService>().To<MyImageService>().AsTransient();
            Container.Bind<IMyPlaceService>().To<MyPlaceService>().AsTransient();
            Container.Bind<ICinemaService>().To<CinemaService>().AsTransient();
            Container.Bind<IAccountsService>().To<AccountsService>().AsSingle();
            Container.Bind<IUsersInSpacesService>().To<UsersInSpacesService>().AsTransient();
            Container.Bind<ITowerEventService>().To<TowerEventService>().AsTransient();
            Container.Bind<ITowerNewsService>().To<TowerNewsService>().AsTransient();
            Container.Bind<ITicketService>().To<TicketService>().AsTransient();
            Container.Bind<ILikeService>().To<LikeService>().AsTransient();
            Container.Bind<IWalletService>().To<WalletService>().AsSingle();
            Container.Bind<ITransactionsService>().To<TransactionsService>().AsSingle();
            Container.Bind<IOculusAuthService>().To<OculusAuthService>().AsSingle();
            Container.Bind<ITradeService>().To<TradeService>().AsSingle();
            Container.Bind<IAppPurchaseService>().To<MetaInAppPurchaseService>().AsSingle();
            Container.Bind<ITowerObjectsClassService>().To<TowerObjectClassService>().AsSingle();
            Container.Bind<ITowerObjectsRevision>().To<TowerObjectRevisionService>().AsSingle();
            Container.Bind<IRegistrationService>().To<RegisterService>().AsTransient();
            Container.Bind<IFriendsService>().To<FriendsService>().AsTransient();
            Container.Bind<ISpacePurchaseService>().To<SpacePurchaseService>().AsTransient();
            Container.Bind<IMafiaGameService>().To<MafiaGameService>().AsTransient();
            Container.Bind<IAvatarRecorderService>().To<AvatarRecorderService>().AsTransient();
        }

        public T Resolve<T>()
        {
            return Container.Resolve<T>();
        }
    }
}