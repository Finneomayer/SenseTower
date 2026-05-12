using Mechanics.Network.Scripts.StaticObjectsService;
using Mechanics.SpaceStaticObjectEditing;
using Mechanics.Network.Scripts.SpaceObjectsService;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Shared
{
    public class PersonalSpaceDIInstaller : MonoInstaller
    {
        [SerializeField] private SpaceFactory _spaceFactory;
        [SerializeField] private SpaceStaticObjectsRepository _spaceRepository;
        [SerializeField] private SpaceEditingService _spaceEditingService;
        
        public override void InstallBindings()
        {
            Container.Bind<ISpaceFactory>().FromInstance(_spaceFactory).AsSingle();
            Container.Bind<ISpaceRepository>().FromInstance(_spaceRepository).AsSingle();
            Container.Bind<ISpaceEditingService>().FromInstance(_spaceEditingService).AsSingle();
            Container.Bind<IStaticObjectsService>().To<StaticObjectsService>().AsTransient();
            Container.Bind<ISpaceObjectService>().To<SpaceObjectService>().AsTransient();
        }
    }
}