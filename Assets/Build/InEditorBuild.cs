#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Space;
using UnityEditor;
using UnityEditor.Build.Reporting;
// using UnityEditor.XR.Management;
// using UnityEditor.XR.Management.Metadata;
using UnityEngine;

namespace Assets.Build
{
    /// 
    /// Put me inside an Editor folder
    /// 
    /// Add a Build menu on the toolbar to automate multiple build for different platform
    /// 
    /// Use #define BUILD in your code if you have build specification 
    /// Specify all your Target to build All
    /// 
    /// Install to Android device using adb install -r "pathofApk"
    /// 
    public class InEditorBuild : MonoBehaviour
    {
        private static readonly ISpaceService SpaceService = new SpaceService();

        private static string[] ExcludedScenes = { "TheEnterScene" };


        static readonly BuildTarget[] TargetToBuildAll =
        {
            BuildTarget.Android,
            BuildTarget.StandaloneWindows64,
            BuildTarget.StandaloneLinux64
        };

        public static string ProductName => PlayerSettings.productName;

        // Helper function for getting the command line arguments
        private static string GetArg(string name)
        {
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == name && args.Length > i + 1)
                {
                    Debug.Log("Process parameter: " + args[i]);
                    return args[i + 1];
                }
            }
            return "";
        }


        private static string BuildPathRoot
        {
            get
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), ProductName);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        static int AndroidLastBuildVersionCode
        {
            get => PlayerPrefs.GetInt("LastVersionCode", -1);
            set => PlayerPrefs.SetInt("LastVersionCode", value);
        }

        static BuildTargetGroup ConvertBuildTarget(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneLinux64:
                    return BuildTargetGroup.Standalone;
                case BuildTarget.Android:
                    return BuildTargetGroup.Android;
                case BuildTarget.WebGL:
                    return BuildTargetGroup.WebGL;
                case BuildTarget.NoTarget:
                default:
                    return BuildTargetGroup.Standalone;
            }
        }
        static string GetExtension(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneOSX:
                    break;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return ".exe";
                case BuildTarget.Android:
                    return ".apk";
                case BuildTarget.StandaloneLinux64:
                    return "x86_x64";
                case BuildTarget.WebGL:
                    break;
                case BuildTarget.NoTarget:
                    break;
            }

            return "";
        }

        static BuildPlayerOptions GetServerPlayerOptions(string scene)
        {
            return new BuildPlayerOptions
            {
                subtarget = (int)StandaloneBuildSubtarget.Server,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.BuildScriptsOnly,
                scenes = new []{scene}
            };
        }

        private static BuildPlayerOptions GetClientPlayerOptions(string[] scenes)
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                options = BuildOptions.None
            };

            // To define
            // buildPlayerOptions.locationPathName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\LightGunBuild\\Android\\LightGunMouseArcadeRoom.apk";
            // buildPlayerOptions.target = BuildTarget.Android;

            return buildPlayerOptions;
        }

        static BuildResult DefaultBuild(BuildTarget buildTarget, bool isServer, string[] scenes, DateTime dt)
        {
            XRPluginManagementSettings.SetValidOpenXRState(buildTarget);

            var dirName = ProductName;
            if (isServer)
            {
                
                dirName = Path.GetFileName(scenes.First());
            }

            var targetGroup = ConvertBuildTarget(buildTarget);

            // Set build output
            string BuildOutputPath = GetArg("-BuildOutputPath");
            var path = "";
            Debug.Log("Output directory: " + BuildOutputPath);

            if (string.IsNullOrEmpty(BuildOutputPath))
            {
                path = Path.Combine(
                    BuildPathRoot,
                    $"Build {dt:dd.MM.yyyy_HH}h{dt:mm}m",
                    buildTarget.ToString(),
                    dirName + (isServer?"server":"client"));
            }
            else {
                path = Path.Combine(
                    BuildOutputPath,
                    dirName);
            }
            
            if (isServer)
            {
                path = Path.Combine(path, "build"); // for scripts simplification
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Debug.Log("Build target dir: " + path);

            string name = (buildTarget == BuildTarget.StandaloneLinux64)? "linux_build.x86_64" : ProductName + GetExtension(buildTarget);

            string defineSymbole = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineSymbole + ";BUILD");

            BuildPlayerOptions buildPlayerOptions = isServer?GetServerPlayerOptions(scenes.First()):GetClientPlayerOptions(scenes);

            buildPlayerOptions.locationPathName = Path.Combine(path, name);
            buildPlayerOptions.target = buildTarget;
            
            Debug.Log("Build location: " + Path.Combine(path, name));

            EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, buildTarget);

            BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

            string result = buildPlayerOptions.locationPathName + ": " + buildReport;
            Debug.Log(result);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineSymbole);

            // not try to open destination directory if build is headless
            if (!Application.isBatchMode) {
                EditorUtility.RevealInFinder(path);
            }

            return buildReport.summary.result;
        }

        [MenuItem("Build/Build Specific/Build VR")]
        static void BuildAndroid()
        {
            // Android build settings
            string AndroidKeystorePass = GetArg("-AndroidKeystorePass");
            string AndroidKeyaliasName = GetArg("-AndroidKeyaliasName");
            string AndroidKeyaliasPass = GetArg("-AndroidKeyaliasPass");
            string AndroidKeystorePath = GetArg("-AndroidKeystorePath");

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystorePass = AndroidKeystorePass;
            PlayerSettings.Android.keyaliasName = AndroidKeyaliasName;
            PlayerSettings.Android.keyaliasPass = AndroidKeyaliasPass;
            PlayerSettings.Android.keystoreName = AndroidKeystorePath;
            AndroidLastBuildVersionCode = PlayerSettings.Android.bundleVersionCode;
            
            DefaultBuild(BuildTarget.Android, false, GetAllScenes(false), DateTime.Now);
        }

        private static string[] GetAllScenes(bool onlyServerScenes)
        {
            var editorScenes = EditorBuildSettings.scenes;

            return editorScenes.Where(es =>
                    ExcludedScenes.Any(excludeScene =>
                        (onlyServerScenes && !es.path.ToLowerInvariant().Contains(excludeScene.ToLowerInvariant())) || !onlyServerScenes))
                .Select(s => s.path)
                .ToArray();;
        }

        [MenuItem("Build/Build Specific/Build Linux Servers")]
        static void BuildLinux()
        {
            var scenes = GetAllScenes(true);
            var dt = DateTime.Now;
            foreach (var scene in scenes)
            {
                Debug.Log("Building Scene: " + scene);
                BuildResult buildResult = DefaultBuild(BuildTarget.StandaloneLinux64, true, new[]{scene}, dt);

                if (buildResult == BuildResult.Failed)
                {
                    EditorApplication.Exit(1);
                    return;
                }
            }
        }

        [MenuItem("Build/Build Specific/Build Windows Servers")]
        static void BuildWindows()
        {
            var scenes = GetAllScenes(true);
            var dt = DateTime.Now;
            foreach (var scene in scenes)
            {
                DefaultBuild(BuildTarget.StandaloneWindows64, true, new[] { scene }, dt);
            }
        }

        [MenuItem("Build/Get Build Number")]
        static void BuildNumber()
        {
            Debug.Log("Current/Last: " + PlayerSettings.Android.bundleVersionCode + "/" + AndroidLastBuildVersionCode);
        }

        [MenuItem("Build/Build Number/Up Build Number")]
        static void BuildNumberUp()
        {
            PlayerSettings.Android.bundleVersionCode++;
            BuildNumber();
        }

        [MenuItem("Build/Build Number/Down Build Number")]
        static void BuildNumberDown()
        {
            PlayerSettings.Android.bundleVersionCode--;
            BuildNumber();
        }

        [MenuItem("Build/Build All")]
        static void BuildAll()
        {
            var clientScenes = GetAllScenes(false);
            var dt = DateTime.Now;
            DefaultBuild(BuildTarget.Android, false, clientScenes, dt);

            var serverScenes = GetAllScenes(true);

            foreach (var scene in serverScenes)
            {
                DefaultBuild(BuildTarget.StandaloneLinux64, true, new[] { scene }, dt);
                DefaultBuild(BuildTarget.StandaloneWindows64, true, new[] { scene }, dt);
            }
        }
    }
}

#endif