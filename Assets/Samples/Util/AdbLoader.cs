using System.Runtime.InteropServices;
using Pico.Platform;

public class AdbLoader
{
    [DllImport("pxrplatformloader", EntryPoint = "ppf_AdbLoaderInit", CallingConvention = CallingConvention.Cdecl)]
    public static extern PlatformInitializeResult ppf_AdbLoaderInit([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaller))] string appId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaller))] string configJson);

    [DllImport("pxrplatformloader", EntryPoint = "ppf_AdbLoaderInitAsynchronous", CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong ppf_AdbLoaderInitAsynchronous([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaller))] string appId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaller))] string configJson);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LogFunction(string logText, int level);

    [DllImport("pxrplatformloader", EntryPoint = "ppf_SetUnityLog", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ppf_SetUnityLog(LogFunction logFun);
}