using Assets.Mechanics.FriendsList;
using Assets.Mechanics.NetworkInteraction.Services;
using Assets.Mechanics.Tips;
using Mechanics.FriendsList;
using Mechanics.SignalBusModels;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Shared
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] private GrabInteractionService _grabInteractionService;
        [SerializeField] private NetworkFriendListService _networkFriendListService;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<AddToFriendListRequestSignal>();
            Container.DeclareSignal<RemoveFromFriendListRequestSignal>();
            
            Container.Bind<IGrabInteraction>().FromInstance(_grabInteractionService).AsSingle();
            Container.Bind<NetworkFriendListService>().FromInstance(_networkFriendListService).AsSingle();
            
            Container.Bind<ITipsSceneRepository>().To<TipsSceneRepository>().AsSingle();
            Container.Bind(typeof(ITipsSceneContext),typeof(IInitializable)).To<TipsSceneContext>().AsSingle();
        }

        public T Resolve<T>()
        {
            return Container.Resolve<T>();
        }
    }
}