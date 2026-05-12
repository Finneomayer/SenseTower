using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Hall;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IMyPlaceService
{
    public static event Action UpdateMyPlaceDataEvent;
    public void UpdateMyPlaceData();
    public UniTask<LocalSpace[]> GetAllMySpaces();
    public UniTask<bool> ReplaceAllMyPlacePictures(Guid myPlaceId, Dictionary<int, MyImage> myPlaceImages);
    public UniTask<bool> UpdateDoorImage(Guid myPlaceId, Guid myImageId);
    public UniTask<bool> ResetDoorImage(Guid myPlaceId);
    public UniTask<bool> AddAdmin(Guid spaceId, Guid userId);
    public UniTask<bool> UpdateAdminList(Guid spaceId, List<string> userIds);
    public UniTask<string[]> GetBlockList(Guid spaceId);
    public UniTask<bool> AddToBlockList(Guid spaceId, Guid userId);
    public UniTask<bool> DeleteFromBlockList(Guid spaceId, Guid userId);
}
