using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSetLoader : MonoBehaviour
{
    [SerializeField]
    private SampleAvatarEntity _avatarPrefab;
    [SerializeField]
    private Transform _parent;

    private void Start()
    {
        const int AvatarCount = 32;
        const int StepX = 2;

        Vector3 currentPosition = _parent.position;
        currentPosition.x -= StepX * AvatarCount / 2;

        for (int i = 0; i < AvatarCount; i++)
        {
            SampleAvatarEntity avatar = Instantiate(_avatarPrefab, currentPosition, Quaternion.identity, _parent);
            avatar.gameObject.name = i.ToString();
            avatar.ReloadAvatarManually(i.ToString(), SampleAvatarEntity.AssetSource.StreamingAssets);
            currentPosition.x += StepX;
        }
    }
}
