using System;
using System.Collections.Generic;
using API.Models;
using Assets.Scripts.Cinema;
using Assets.Scripts.Event;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;

public interface IServerApiService
{
    public UniTask Initialize();
    public event Action ServerAuth;
    public UniTask SendServerUsers(RegisterUsersInSpaceData usersInSpaceData);
    public UniTask<LocalSpace> GetPlaceBySpace(string spaceId);
    public UniTask<Cinema> GetCinemaById(string id);
    public UniTask<TowerEvent[]> GetTowerEvents(TowerEventsFilter filter);
    
}
