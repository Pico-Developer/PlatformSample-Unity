using System;
using Unity.XR.PXR;
using UnityEngine;

namespace Pico.Platform.Samples
{
    public class SdkUtil
    {
        public static string GetAppID(string appId = null)
        {
            string configAppID = PXR_PlatformSetting.Instance.appID.Trim();
            if (String.IsNullOrWhiteSpace(appId))
            {
                if (String.IsNullOrWhiteSpace(configAppID))
                {
                    throw new UnityException("Cannot find appId");
                }

                appId = configAppID;
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(configAppID))
                {
                    Debug.LogWarning($"Using {appId} as appId rather than {configAppID} which is configured in Unity Editor");
                }
            }

            return appId;
        }

        public static void SetOnLaunchApp(Action<string> callback)
        {
            ApplicationService.SetLaunchIntentChangedCallback(m =>
            {
                var launchDetails = ApplicationService.GetLaunchDetails();
                if (launchDetails.LaunchType == LaunchType.Deeplink && string.IsNullOrWhiteSpace(launchDetails.DestinationApiName))
                {
                    callback.Invoke(launchDetails.DeeplinkMessage);
                }
            });
        }

        public static void Initialize(bool async, Action onSuccess, Action onFail)
        {
            if (async)
            {
                CoreService.AsyncInitialize().OnComplete(m =>
                {
                    if (m.IsError)
                    {
                        Debug.Log($"AsyncInitialize failed code={m.Error.Code} message={m.Error.Message}");
                        onFail();
                        return;
                    }

                    if (m.Data != PlatformInitializeResult.Success && m.Data != PlatformInitializeResult.AlreadyInitialized)
                    {
                        Debug.Log($"Async Initialize failed {m.Data}");
                        onFail();
                        return;
                    }

                    onSuccess();
                });
            }
            else
            {
                try
                {
                    CoreService.Initialize();
                }
                catch (Exception e)
                {
                    onFail();
                    return;
                }

                onSuccess();
            }
        }
    }
}