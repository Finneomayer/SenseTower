#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;

namespace Assets.Build
{
    public class XRActiveBuildTargetListener : IActiveBuildTargetChanged
    {
        public int callbackOrder => 0;

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            XRPluginManagementSettings.SetValidOpenXRState(newTarget);
        }
    }
}

#endif
