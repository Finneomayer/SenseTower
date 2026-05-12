using static Data.Enumenators;

namespace Mechanics.LoadSceneObjects.Models
{
    public class SceneObject
    {
        public string RerositoryUrl { get; set; }
        public RemoteContentType ObjectType { get; set; } = RemoteContentType.Unknown;
        public PrefabObjectType PrefabObjectType { get; set; } = PrefabObjectType.Unknown;
        public string ObjectKey { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Xr { get; set; }
        public float Yr { get; set; }
        public float Zr { get; set; }
        public float Xs { get; set; }
        public float Ys { get; set; }
        public float Zs { get; set; }
    }
}
