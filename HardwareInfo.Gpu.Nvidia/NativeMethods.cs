namespace HardwareInfo.Gpu.Nvidia;

using System.Runtime.InteropServices;

using static HardwareInfo.Gpu.Nvidia.NativeMethods;

// https://docs.nvidia.com/deploy/nvml-api/group__nvmlDeviceQueries.html
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
#pragma warning disable CA5392
internal static class NativeMethods
{
    //------------------------------------------------------------------------
    // Enum
    //------------------------------------------------------------------------

    public enum NvmlReturn
    {
        Success = 0,
        Uninitialized = 1,
        InvalidArgument = 2,
        NotSupported = 3,
        NoPermission = 4,
        NotFound = 6,
        InsufficientSize = 7,
        InsufficientPower = 8,
        DriverNotLoaded = 9,
        TimeOut = 10,
        IRQIssue = 11,
        LibraryNotFound = 12,
        FunctionNotFound = 13,
        CorruptedInfoRom = 14,
        GpuIsLost = 15,
        ResetRequired = 16,
        OperatingSystem = 17,
        LibRmVersionMismatch = 18,
        InUse = 19,
        Unknown = 999
    }

    public enum NvmlTemperatureSensors
    {
        Gpu = 0
    }

    public enum NvmlClockType
    {
        Graphics = 0,
        Sm = 1,
        Mem = 2,
        Video = 3
    }

    //------------------------------------------------------------------------
    // Struct
    //------------------------------------------------------------------------

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NvmlUtilization
    {
        public uint Gpu;
        public uint Memory;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NvmlMemory
    {
        public ulong Total;
        public ulong Free;
        public ulong Used;
    }

    //------------------------------------------------------------------------
    // Method
    //------------------------------------------------------------------------

    private const string NvmlDll = "nvml.dll";

    [DllImport(NvmlDll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "nvmlInit_v2")]
    public static extern NvmlReturn NvmlInit();

    [DllImport(NvmlDll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "nvmlShutdown")]
    public static extern NvmlReturn NvmlShutdown();

    [DllImport(NvmlDll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "nvmlDeviceGetCount_v2")]
    public static extern NvmlReturn NvmlDeviceGetCount(out uint deviceCount);

    [DllImport(NvmlDll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "nvmlDeviceGetHandleByIndex_v2")]
    public static extern NvmlReturn NvmlDeviceGetHandleByIndex(uint index, out IntPtr device);

    // Utilization

    [DllImport(NvmlDll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "nvmlDeviceGetUtilizationRates")]
    public static extern NvmlReturn NvmlDeviceGetUtilizationRates(IntPtr device, out NvmlUtilization utilization);

    // Memory

    [DllImport(NvmlDll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "nvmlDeviceGetMemoryInfo")]
    public static extern NvmlReturn NvmlDeviceGetMemoryInfo(IntPtr device, out NvmlMemory memory);

    // Power

    [DllImport(NvmlDll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "nvmlDeviceGetPowerUsage")]
    public static extern NvmlReturn NvmlDeviceGetPowerUsage(IntPtr device, out uint power);

    [DllImport(NvmlDll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "nvmlDeviceGetEnforcedPowerLimit")]
    public static extern NvmlReturn NvmlDeviceGetEnforcedPowerLimit(IntPtr device, out uint powerLimit);

    // Temperature

    [DllImport(NvmlDll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "nvmlDeviceGetTemperature")]
    public static extern NvmlReturn NvmlDeviceGetTemperature(IntPtr device, NvmlTemperatureSensors sensorType, out uint temp);

    // Clock

    [DllImport(NvmlDll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "nvmlDeviceGetClockInfo")]
    public static extern NvmlReturn NvmlDeviceGetClockInfo(IntPtr device, NvmlClockType clockType, out uint clock);

    // Fan


    [DllImport("nvml.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "nvmlDeviceGetNumFans")]
    public static extern NvmlReturn NvmlDeviceGetNumFans(IntPtr device, out uint fanCount);

    [DllImport(NvmlDll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "nvmlDeviceGetFanSpeed_v2")]
    public static extern NvmlReturn NvmlDeviceGetFanSpeed(IntPtr device, uint index, out uint speed);
}
#pragma warning restore CA5392
// ReSharper restore InconsistentNaming
// ReSharper restore IdentifierTypo
// ReSharper restore CommentTypo
