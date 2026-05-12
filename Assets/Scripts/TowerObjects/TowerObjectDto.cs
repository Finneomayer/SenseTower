using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Assets.Scripts.TowerObjects
{
    public record TowerObjectDto
    {
        public Guid Id;
        public Guid ObjectClassId { get; set; }

        public string? Name;
        public string? Description;
        public string? TowerObjectClassName;

        //public TowerObjectBehaviorType BehaviorType;

        public Enumenators.PrefabObjectType? PrefabObjectType;

        public RemoteObjectTypeInfo? RemoteObjectTypeInfo;

        public Guid? OwnerId;
        public string? OwnerName;
        public BusinessUnitType? OwnerBusinessUnitType;

        public string ObjectKey => GetObjectKey();
        public LoadingObjectType LoadingObjectType => GetLoadingObjectType();

        public LoadingObjectType GetLoadingObjectType()
        {
            if (RemoteObjectTypeInfo != null && !string.IsNullOrEmpty(RemoteObjectTypeInfo.ObjectRepositoryUrl))
            {
                return LoadingObjectType.Remote;
            }
            return LoadingObjectType.Prefab;
        }

        public string GetObjectKey()
        {
            if (RemoteObjectTypeInfo != null)
            {
                return RemoteObjectTypeInfo.ObjectKey;
            }
            return null;
        }

        public bool IsEqual(TowerObjectDto towerObjectDto)
        {
            if (towerObjectDto.Id != Id
                || towerObjectDto.ObjectClassId != ObjectClassId
                || towerObjectDto.Name != Name
                || towerObjectDto.Description != Description
                || towerObjectDto.TowerObjectClassName != TowerObjectClassName
                //|| towerObjectDto.BehaviorType != BehaviorType
                || towerObjectDto.LoadingObjectType != LoadingObjectType
                || towerObjectDto.PrefabObjectType != PrefabObjectType
                || towerObjectDto.OwnerId != OwnerId
                || towerObjectDto.OwnerName != OwnerName
                || towerObjectDto.OwnerBusinessUnitType != OwnerBusinessUnitType)
            {
                return false;
            }

            if (towerObjectDto.RemoteObjectTypeInfo == null && RemoteObjectTypeInfo != null
                || towerObjectDto.RemoteObjectTypeInfo != null && RemoteObjectTypeInfo == null)
            {
                return false;
            }

            if (towerObjectDto.RemoteObjectTypeInfo != null
                && (towerObjectDto.RemoteObjectTypeInfo.ObjectKey != RemoteObjectTypeInfo.ObjectKey
                || towerObjectDto.RemoteObjectTypeInfo.ObjectRepositoryUrl != RemoteObjectTypeInfo.ObjectRepositoryUrl))
            {
                return false;
            }

            return true;
        }

    };

    public class TowerObjectDtoComparer : IEqualityComparer<TowerObjectDto>
    {
        private IEqualityComparer<TowerObjectDto> _equalityComparerImplementation;

        public bool Equals(TowerObjectDto x, TowerObjectDto y)
        {
            if (x == null || y == null) return false;
            if (object.ReferenceEquals(x, y)) return true;
            return x.Name == y.Name && x.ObjectKey == y.ObjectKey;
        }

        public int GetHashCode(TowerObjectDto obj)
        {
            return (obj.Name + ";" + obj.ObjectKey).GetHashCode();
        }
    }
}