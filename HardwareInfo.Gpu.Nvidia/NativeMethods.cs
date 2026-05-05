namespace HardwareInfo.Gpu.Nvidia;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// https://docs.nvidia.com/deploy/nvml-api/group__nvmlDeviceQueries.html
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
#pragma warning disable CA5392
internal static partial class NativeMethods
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

    public enum NvmlPcieUtilCounter
    {
        TxBytes = 0,
        RxBytes = 1
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

    [LibraryImport(NvmlDll, EntryPoint = "nvmlInit_v2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlInit();

    [LibraryImport(NvmlDll, EntryPoint = "nvmlShutdown")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlShutdown();

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetCount_v2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetCount(out uint deviceCount);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetHandleByIndex_v2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetHandleByIndex(uint index, out IntPtr device);

    // Utilization

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetUtilizationRates")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetUtilizationRates(IntPtr device, out NvmlUtilization utilization);

    // Memory

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetMemoryInfo")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetMemoryInfo(IntPtr device, out NvmlMemory memory);

    // Power

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetPowerUsage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetPowerUsage(IntPtr device, out uint power);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetEnforcedPowerLimit")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetEnforcedPowerLimit(IntPtr device, out uint powerLimit);

    // Temperature

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetTemperature")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetTemperature(IntPtr device, NvmlTemperatureSensors sensorType, out uint temp);

    // Clock

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetClockInfo")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetClockInfo(IntPtr device, NvmlClockType clockType, out uint clock);

    // Fan

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetNumFans")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetNumFans(IntPtr device, out uint fanCount);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetFanSpeed_v2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetFanSpeed(IntPtr device, uint index, out uint speed);

    // PCIe

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetPcieThroughput")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetPcieThroughput(IntPtr device, NvmlPcieUtilCounter counter, out uint value);
}
#pragma warning restore CA5392
// ReSharper restore InconsistentNaming
// ReSharper restore IdentifierTypo
// ReSharper restore CommentTypo
