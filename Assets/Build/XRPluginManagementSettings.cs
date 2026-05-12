#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;

namespace Assets.Build
{
    // ref. https://docs.unity3d.com/Packages/com.unity.xr.management@4.1/manual/EndUser.html
    public static class XRPluginManagementSettings
    {
        public enum Plugin
        {
            OpenXR,
            Oculus,
            OpenVR
        }

        public static void EnablePlugin(BuildTargetGroup buildTargetGroup, Plugin plugin)
        {
            var buildTargetSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
            var pluginsSettings = buildTargetSettings.AssignedSettings;
            var success = XRPackageMetadataStore.AssignLoader(pluginsSettings, GetLoaderName(plugin), buildTargetGroup);
            if (success)
            {
                Debug.Log($"XR Plug-in Management: Enabled {plugin} plugin on {buildTargetGroup}");
            }
        }

        public static void DisablePlugin(BuildTargetGroup buildTargetGroup, Plugin plugin)
        {
            var buildTargetSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
            var pluginsSettings = buildTargetSettings.AssignedSettings;
            var success = XRPackageMetadataStore.RemoveLoader(pluginsSettings, GetLoaderName(plugin), buildTargetGroup);
            if (success)
            {
                Debug.Log($"XR Plug-in Management: Disabled {plugin} plugin on {buildTargetGroup}");
            }
        }

        public static void SetValidOpenXRState(BuildTarget buildTarget)
        {
            bool needEnableXR;
            BuildTargetGroup buildTargetGroup;

            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows64:
                    needEnableXR = true;
                    buildTargetGroup = BuildTargetGroup.Standalone;
                    break;
                case BuildTarget.Android:
                    needEnableXR = false;
                    buildTargetGroup = BuildTargetGroup.Android;
                    break;
                case BuildTarget.StandaloneLinux64:
                    needEnableXR = false;
                    buildTargetGroup = BuildTargetGroup.Standalone;
                    break;
                default:
                    needEnableXR = false;
                    buildTargetGroup = BuildTargetGroup.Unknown;
                    break;
            }

            if (buildTargetGroup == BuildTargetGroup.Unknown)
            {
                return;
            }

            if (needEnableXR)
            {
                EnablePlugin(buildTargetGroup, Plugin.OpenXR);
            }
            else
            {
                DisablePlugin(buildTargetGroup, Plugin.OpenXR);
            }
        }

        public static string GetLoaderName(Plugin plugin) => plugin switch
        {
            Plugin.OpenXR => "UnityEngine.XR.OpenXR.OpenXRLoader",
            Plugin.Oculus => "Unity.XR.Oculus.OculusLoader",
            Plugin.OpenVR => "Unity.XR.OpenVR.OpenVRLoader",
            _ => throw new NotImplementedException()
        };
    }
}

#endif
