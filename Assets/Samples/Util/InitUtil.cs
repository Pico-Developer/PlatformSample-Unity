using System;
using System.IO;
using AOT;
using LitJson;
using Pico.Platform;
using Pico.Platform.Framework;
using UnityEngine;

namespace PICO.Platform.Samples
{
    public class InitUtil
    {
        [MonoPInvokeCallback(typeof(AdbLoader.LogFunction))]
        static void unityLog(string logText, int level)
        {
            switch (level)
            {
                case 0:
                {
                    Debug.Log(logText);
                    break;
                }
                case 1:
                {
                    Debug.LogWarning(logText);
                    break;
                }
                case 2:
                {
                    Debug.LogError(logText);
                    break;
                }
            }
        }

        /// <summary>
        /// Initializes the Platform SDK asynchronously.
        /// </summary>
        /// <param name="appId">The app ID for the Platform SDK. If not provided, Unity editor configuration will be applied.</param>
        /// <returns>The initialization result.</returns>
        /// <exception cref="UnityException">If the input app ID is null or empty or if the initialization fails, this exception will be thrown.</exception>
        /// <exception cref="NotImplementedException">If the current platform is not supported, this exception will be thrown.</exception>
        public static Task<PlatformInitializeResult> AsyncInitialize(string appId = null)
        {
            appId = CoreService.GetAppID(appId);
            if (String.IsNullOrWhiteSpace(appId))
            {
                throw new UnityException("AppID cannot be null or empty");
            }

            Task<PlatformInitializeResult> task;
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                ulong requestId = CLIB.ppf_InitializeAndroidAsynchronous(appId, activity.GetRawObject(), IntPtr.Zero);
                Debug.Log($"ppf_InitializeAndroidAsynchronous2 {requestId}");
                if (requestId == 0)
                {
                    throw new Exception("PICO PlatformSDK failed to initialize");
                }
                else
                {
                    task = new Task<PlatformInitializeResult>(requestId);
                }
            }
            else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                AdbLoader.ppf_SetUnityLog(unityLog);
                JsonData config = new JsonData();
                var configJsonString = Resources.Load<TextAsset>("AdbLoader");
                if (configJsonString == null)
                {
                    throw new UnityException($"cannot find AdbLoader config file :Assets/Resources/AdbLoader.json");
                }

                var configJson = JsonMapper.ToObject(configJsonString.text);
                config["port"] = configJson["port"];
                config["adbPath"] = configJson["adbPath"];
                var requestId = AdbLoader.ppf_AdbLoaderInitAsynchronous(appId, config.ToJson());
                if (requestId == 0)
                {
                    throw new Exception("PICO PlatformSDK failed to initialize");
                }
                else
                {
                    task = new Task<PlatformInitializeResult>(requestId);
                }
            }
            else if ((Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor))
            {
                var config = Resources.Load<TextAsset>("PicoSdkPCConfig");
                var logDirectory = Path.GetFullPath("Logs");
                if (config == null)
                {
                    throw new UnityException($"cannot find PC config file Resources/PicoSdkPCConfig");
                }

                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                var requestId = CLIB.ppf_PcInitAsynchronousWrapper(appId, config.text, logDirectory);
                if (requestId == 0)
                {
                    throw new Exception("PICO PlatformSDK failed to initialize");
                }
                else
                {
                    task = new Task<PlatformInitializeResult>(requestId);
                }
            }
            else
            {
                throw new NotImplementedException("PICO platform is not implemented on this platform yet.");
            }

            CoreService.Initialized = true;
            Runner.RegisterGameObject();
            return task;
        }

        /// <summary>
        /// Initializes the Platform SDK synchronously.
        /// </summary>
        /// <param name="appId">The app ID for the Platform SDK. If not provided, Unity editor configuration will be applied.</param>
        /// <exception cref="NotImplementedException">If the current platform is not supported, this exception will be thrown.</exception>
        /// <exception cref="UnityException">If the initialization fails, this exception will be thrown.</exception>
        public static void Initialize(string appId = null)
        {
            if (CoreService.Initialized)
            {
                return;
            }

            appId = CoreService.GetAppID(appId);
            if (String.IsNullOrWhiteSpace(appId))
            {
                throw new UnityException("AppID must not be null or empty");
            }

            PlatformInitializeResult initializeResult;
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");

                initializeResult = CLIB.ppf_InitializeAndroid(appId, activity.GetRawObject(), IntPtr.Zero);

                if (initializeResult == PlatformInitializeResult.Success ||
                    initializeResult == PlatformInitializeResult.AlreadyInitialized)
                {
                    CoreService.Initialized = true;
                }
            }
            else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                AdbLoader.ppf_SetUnityLog((text, level) => { Debug.Log(text); });
                JsonData config = new JsonData();
                config["port"] = 1234;
                config["adbPath"] = "/usr/local/bin/adb";
                initializeResult = AdbLoader.ppf_AdbLoaderInit(appId, JsonMapper.ToJson(config));
                if (initializeResult == PlatformInitializeResult.Success ||
                    initializeResult == PlatformInitializeResult.AlreadyInitialized)
                {
                    CoreService.Initialized = true;
                }
            }
            else if ((Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor))
            {
                var config = Resources.Load<TextAsset>("PicoSdkPCConfig");
                if (config == null)
                {
                    throw new UnityException($"cannot find PC config file Resources/PicoSdkPCConfig");
                }

                var logDirectory = Path.GetFullPath("Logs");
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                initializeResult = CLIB.ppf_PcInitWrapper(appId, config.text, logDirectory);
                if (initializeResult == PlatformInitializeResult.Success ||
                    initializeResult == PlatformInitializeResult.AlreadyInitialized)
                {
                    CoreService.Initialized = true;
                }
            }
            else
            {
                throw new NotImplementedException("PICO platform is not implemented on this platform yet.");
            }

            if (!CoreService.Initialized)
            {
                throw new UnityException($"PICO Platform failed to initialize：{initializeResult}.");
            }

            Runner.RegisterGameObject();
        }
    }
}