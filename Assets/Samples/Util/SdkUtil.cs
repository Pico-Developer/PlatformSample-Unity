using System;
using UnityEngine;

namespace Pico.Platform.Samples
{
    public class SdkUtil
    {
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