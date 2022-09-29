using Newtonsoft.Json;
using Unity.XR.PXR;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.XR.Management;

namespace PlatformSdk.Editor
{
    class FastRunSettings : ScriptableObject
    {
    }

    /// <summary>
    /// Unity Setting Getter and Setter
    /// </summary>
    class GS
    {
        public static string appId
        {
            get { return PXR_PlatformSetting.Instance.appID; }
            set { PXR_PlatformSetting.Instance.appID = value; }
        }

        public static string productName
        {
            get { return PlayerSettings.productName; }
            set { PlayerSettings.productName = value; }
        }

        public static string packageName
        {
            get { return PlayerSettings.GetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup); }
            set { PlayerSettings.SetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup, value); }
        }

        public static BuildTargetGroup buildTargetGroup
        {
            get { return EditorUserBuildSettings.selectedBuildTargetGroup; }
            set { EditorUserBuildSettings.selectedBuildTargetGroup = value; }
        }

        public static AndroidSdkVersions minSdkVersion
        {
            get { return PlayerSettings.Android.minSdkVersion; }
            set { PlayerSettings.Android.minSdkVersion = value; }
        }

        public static AndroidSdkVersions targetSdkVersion
        {
            get { return PlayerSettings.Android.targetSdkVersion; }
            set { PlayerSettings.Android.targetSdkVersion = value; }
        }

        public static AndroidArchitecture targetArchitectures
        {
            get { return PlayerSettings.Android.targetArchitectures; }
            set { PlayerSettings.Android.targetArchitectures = value; }
        }

        public static string bundleVersion
        {
            get { return PlayerSettings.bundleVersion; }
            set { PlayerSettings.bundleVersion = value; }
        }

        public static int bundleVersionCode
        {
            get { return PlayerSettings.Android.bundleVersionCode; }
            set { PlayerSettings.Android.bundleVersionCode = value; }
        }

        public static ScriptingImplementation scriptBackend
        {
            get { return PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup); }
            set { PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, value); }
        }

        public static AndroidBuildType androidBuildType
        {
            get { return EditorUserBuildSettings.androidBuildType; }
            set { EditorUserBuildSettings.androidBuildType = value; }
        }

        static XRManagerSettings GetXrSettings()
        {
            XRGeneralSettings generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
            if (generalSettings == null) return null;
            var assignedSettings = generalSettings.AssignedSettings;
            return assignedSettings;
        }

        static PXR_Loader GetPxrLoader()
        {
            var x = GetXrSettings();
            if (x == null) return null;
            foreach (var i in x.activeLoaders)
            {
                if (i is PXR_Loader)
                {
                    return i as PXR_Loader;
                }
            }

            return null;
        }

        public static bool UsePicoXr
        {
            get { return GetPxrLoader() != null; }
            set
            {
                var x = GetXrSettings();
                if (x == null) return;
                var loader = GetPxrLoader();
                if (value == false)
                {
                    if (loader == null)
                    {
                    }
                    else
                    {
                        x.TryRemoveLoader(loader);
                    }
                }
                else
                {
                    if (loader == null)
                    {
                        var res = XRPackageMetadataStore.AssignLoader(x, nameof(PXR_Loader), BuildTargetGroup.Android);
                        Debug.Log($"设置XR{res} {value}");
                    }
                    else
                    {
                    }
                }
            }
        }
    }

    [CustomEditor(typeof(FastRunSettings))]
    public class FastRunSettingsEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            Debug.Log($"配置OnEnable");
        }

        public override void OnInspectorGUI()
        {
            var obj = Selection.activeObject as FastRunSettings;
            //appId
            {
                EditorGUILayout.LabelField("App Id");
                GS.appId = EditorGUILayout.TextField(GS.appId);
                EditorGUILayout.Separator();
            }

            //ProductName
            {
                EditorGUILayout.LabelField("Product Name");
                GS.productName = EditorGUILayout.TextField(GS.productName);
                EditorGUILayout.Separator();
            }

            //PackageName
            {
                EditorGUILayout.LabelField("Package Name");
                var x = EditorGUILayout.TextField(GS.packageName);
                PlayerSettings.SetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup, x);
                EditorGUILayout.Separator();
            }


            //Build Target
            {
                EditorGUILayout.LabelField("Build Target");
                GS.buildTargetGroup = (BuildTargetGroup) EditorGUILayout.EnumPopup(GS.buildTargetGroup);
                EditorGUILayout.Separator();
            }


            //Min SDK Version
            {
                EditorGUILayout.LabelField("Min SDK Version");
                GS.minSdkVersion = (AndroidSdkVersions) EditorGUILayout.EnumFlagsField(GS.minSdkVersion);
                EditorGUILayout.Separator();
            }

            //Target SDK Version
            {
                EditorGUILayout.LabelField("Target SDK Version");
                GS.targetSdkVersion = (AndroidSdkVersions) EditorGUILayout.EnumPopup(GS.targetSdkVersion);
                EditorGUILayout.Separator();
            }


            //Target Architectures
            {
                EditorGUILayout.LabelField("Target Architecture");
                GS.targetArchitectures = (AndroidArchitecture) EditorGUILayout.EnumPopup(getFirst(GS.targetArchitectures));
                EditorGUILayout.Separator();
            }


            //Bundle Version
            {
                EditorGUILayout.LabelField("Bundle Version");
                GS.bundleVersion = EditorGUILayout.TextField(GS.bundleVersion);
                EditorGUILayout.Separator();
            }

            //bundleVersionCode
            {
                EditorGUILayout.LabelField("Bundle Version Code");
                GS.bundleVersionCode = EditorGUILayout.IntField(GS.bundleVersionCode);
                EditorGUILayout.Separator();
            }

            //ScriptBackend
            {
                EditorGUILayout.LabelField("Script Backend");
                GS.scriptBackend = (ScriptingImplementation) EditorGUILayout.EnumPopup(GS.scriptBackend);
                EditorGUILayout.Separator();
            }


            //AndroidBuildType
            {
                EditorGUILayout.LabelField("Android Build Type");
                GS.androidBuildType = (AndroidBuildType) EditorGUILayout.EnumPopup(GS.androidBuildType);
                EditorGUILayout.Separator();
            }
            //UsePicoXr
            {
                EditorGUILayout.LabelField("Use Pico XR");
                GS.UsePicoXr = EditorGUILayout.Toggle(GS.UsePicoXr);
            }
            //场景编译
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Build Selected Scene"))
                {
                    MenuConfig.BuildAllScene();
                }

                if (GUILayout.Button("Build Current Scene"))
                {
                    MenuConfig.BuildCurrentScene();
                }

                EditorGUILayout.EndHorizontal();
            }


            EditorGUILayout.Separator();
        }

        private static AndroidArchitecture getFirst(AndroidArchitecture targetArchitectures)
        {
            if (targetArchitectures.HasFlag(AndroidArchitecture.ARM64))
            {
                return AndroidArchitecture.ARM64;
            }

            if (targetArchitectures.HasFlag(AndroidArchitecture.ARMv7))
            {
                return AndroidArchitecture.ARMv7;
            }

            return AndroidArchitecture.None;
        }
    }

    class MenuConfig
    {
        [MenuItem("Fast Run/Config")]
        public static void v2()
        {
            var obj = ScriptableObject.CreateInstance<FastRunSettings>();
            obj.name = "Fast Run";
            Selection.activeObject = obj;
            Debug.Log("配置PlatformSdkEditor");
        }

        [MenuItem("Fast Run/Build Selected Scene")]
        public static void BuildAllScene()
        {
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "a.apk", BuildTarget.Android, BuildOptions.None);
        }

        [MenuItem("Fast Run/Build Current Scene")]
        public static void BuildCurrentScene()
        {
            BuildPipeline.BuildPlayer(new EditorBuildSettingsScene[] { }, "a.apk", BuildTarget.Android, BuildOptions.None);
        }
    }
}