using Assets.Scripts.Space;
using System;
using UnityEngine;

public class CameraData
{
    public string CameraId { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public string YoutubeUrl { get; set; }
    public SpaceType SpaceType { get; set; }
    public string SpaceId { get; set; }
}
