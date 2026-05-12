using System.Collections;
using System.Collections.Generic;
using Unity.XR.Oculus;
using UnityEngine;

public static class ConsoleController
{
    public static int FoveationLevel { get; set; }
    public static SystemHeadset Type { get; set; }
    public static int frameRate { get; set; }
    public static string ZoneInfo { get; set; }
}
