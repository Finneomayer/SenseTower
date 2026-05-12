using Assets.Scripts.Space;
using System;
using System.Collections.Generic;
using Assets.Scripts.Audio;
using UnityEngine;

public class UserPlaceVisitorsUI : MonoBehaviour
{
    [SerializeField] private Transform VisitorItemsContent;
    [SerializeField] private UserPlaceVisitorItem VisitorItemPrefab;

    private List<UserPlaceVisitorItem> _visitorItems = new();
    
    public event Action<UserInSpaceInfoDto> KickRequested;
    public event Action<UserInSpaceInfoDto> MuteRequested;

    private void OnDestroy()
    {
        Clear();
    }

    public void Init(LocalSpace space, List<UserInSpaceInfoDto> visitors, Guid localUserId, bool allowManageUsers, IAudioService audioService)
    {
        Clear();

        if (visitors == null || visitors.Count == 0)
        {
            return;
        }

        List<UserInSpaceInfoDto> orderedVisitors = new();
        UserInSpaceInfoDto localUserVisitor = null;
        UserInSpaceInfoDto ownerVisitor = null;

        for (int i = 0; i < visitors.Count; i++)
        {
            if (visitors[i].UserId == localUserId)
            {
                localUserVisitor = visitors[i];
                if (IsSpaceOwner(space, visitors[i].UserId))
                {
                    ownerVisitor = visitors[i];
                }
            }
            else if (IsSpaceOwner(space, visitors[i].UserId))
            {
                ownerVisitor = visitors[i];
            }
            else
            {
                orderedVisitors.Add(visitors[i]);
            }
        }

        if (ownerVisitor != null)
        {
            orderedVisitors.Insert(0, ownerVisitor);
        }

        if (localUserVisitor != null && localUserVisitor != ownerVisitor)
        {
            orderedVisitors.Insert(0, localUserVisitor);
        }

        foreach (var visitor in orderedVisitors)
        {
            UserPlaceVisitorItem visitorItem = Instantiate(VisitorItemPrefab, VisitorItemsContent);

            visitorItem.Init(visitor, localUserVisitor, ownerVisitor, allowManageUsers);
            visitorItem.ChangeMuteState(audioService.MutedUsersID.ContainsValue(visitor.UserId.ToString()));
            
            visitorItem.KickButtonClicked += OnKickRequested;
            visitorItem.MuteButtonClicked += OnMuteRequested;
            
            _visitorItems.Add(visitorItem);
        }
    }

    public void Clear()
    {
        foreach (var visitorItem in _visitorItems)
        {
            if (visitorItem == null)
            {
                continue;
            }

            visitorItem.KickButtonClicked -= OnKickRequested;
            visitorItem.MuteButtonClicked -= OnMuteRequested;

            Destroy(visitorItem.gameObject);
        }

        _visitorItems.Clear();
    }

    private bool IsSpaceOwner(LocalSpace space, Guid userId)
    {
        if (space == null || space.SpaceOwner == null)
        {
            return false;
        }

        return space.SpaceOwner.UserId == userId;
    }

    private void OnKickRequested(UserPlaceVisitorItem userPlaceVisitorItem)
    {
        KickRequested?.Invoke(userPlaceVisitorItem.Visitor);
    }

    private void OnMuteRequested(UserPlaceVisitorItem userPlaceVisitorItem)
    {
        MuteRequested?.Invoke(userPlaceVisitorItem.Visitor);
    }
}