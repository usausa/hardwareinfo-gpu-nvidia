namespace HardwareInfo.Gpu.Nvidia;

using System.Runtime.InteropServices;

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
        NVML_TEMPERATURE_GPU = 0
    }

    public enum NvmlClockType
    {
        NVML_CLOCK_GRAPHICS = 0,
        NVML_CLOCK_SM = 1,
        NVML_CLOCK_MEM = 2,
        NVML_CLOCK_VIDEO = 3
    }

    //------------------------------------------------------------------------
    // Struct
    //------------------------------------------------------------------------

    [StructLayout(LayoutKind.Sequential)]
    public struct NvmlUtilization
    {
        public uint Gpu;
        public uint Memory;
    }

    [StructLayout(LayoutKind.Sequential)]
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

    [DllImport(NvmlDll, EntryPoint = "nvmlInit_v2")]
    public static extern NvmlReturn NvmlInit();

    [DllImport(NvmlDll, EntryPoint = "nvmlShutdown")]
    public static extern NvmlReturn NvmlShutdown();

    [DllImport(NvmlDll, EntryPoint = "nvmlDeviceGetCount_v2")]
    public static extern NvmlReturn NvmlDeviceGetCount(out uint deviceCount);

    [DllImport(NvmlDll, EntryPoint = "nvmlDeviceGetHandleByIndex_v2")]
    public static extern NvmlReturn NvmlDeviceGetHandleByIndex(uint index, out IntPtr device);

    [DllImport(NvmlDll, EntryPoint = "nvmlDeviceGetUtilizationRates")]
    public static extern NvmlReturn NvmlDeviceGetUtilizationRates(IntPtr device, out NvmlUtilization utilization);

    [DllImport(NvmlDll, EntryPoint = "nvmlDeviceGetTemperature")]
    public static extern NvmlReturn NvmlDeviceGetTemperature(IntPtr device, NvmlTemperatureSensors sensorType, out uint temp);

    [DllImport(NvmlDll, EntryPoint = "nvmlDeviceGetMemoryInfo")]
    public static extern NvmlReturn NvmlDeviceGetMemoryInfo(IntPtr device, out NvmlMemory memory);

    [DllImport(NvmlDll, EntryPoint = "nvmlDeviceGetClockInfo")]
    public static extern NvmlReturn NvmlDeviceGetClockInfo(IntPtr device, NvmlClockType clockType, out uint clock);

    [DllImport(NvmlDll, EntryPoint = "nvmlDeviceGetPowerUsage")]
    public static extern NvmlReturn NvmlDeviceGetPowerUsage(IntPtr device, out uint power);

    [DllImport(NvmlDll, EntryPoint = "nvmlDeviceGetFanSpeed_v2")]
    public static extern NvmlReturn NvmlDeviceGetFanSpeed(IntPtr device, uint index, out uint speed);

    [DllImport(NvmlDll, EntryPoint = "nvmlDeviceGetEnforcedPowerLimit")]
    public static extern NvmlReturn NvmlDeviceGetEnforcedPowerLimit(IntPtr device, out uint powerLimit);
}
#pragma warning restore CA5392
// ReSharper restore InconsistentNaming
// ReSharper restore IdentifierTypo
// ReSharper restore CommentTypo
