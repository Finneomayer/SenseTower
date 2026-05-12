using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class MyImage
{
    public Guid Id { get; set; } = default;
    public Guid? PlaceId { get; set; }
    public string PlaceName { get; set; }
    public string Name { get; set; }
    public string FileUrl { get; set; } = null;
    public string PreviewUrl { get; set; } = null;
}
