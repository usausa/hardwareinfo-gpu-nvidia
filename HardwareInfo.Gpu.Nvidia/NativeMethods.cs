namespace HardwareInfo.Gpu.Nvidia;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

// https://docs.nvidia.com/deploy/nvml-api/group__nvmlDeviceQueries.html
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo
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

    public enum NvmlTemperatureSensors : uint
    {
        Gpu = 0,
        Memory = 1
    }

    public enum NvmlBrandType : uint
    {
        Unknown = 0,
        Quadro = 1,
        Tesla = 2,
        NVS = 3,
        Grid = 4,
        GeForce = 5,
        Titan = 6,
        NvidiaVapps = 7,
        GeForceRtx = 8,
        Titan2 = 9
    }

    public enum NvmlDeviceArchitecture : uint
    {
        Kepler = 2,
        Maxwell = 3,
        Pascal = 4,
        Volta = 5,
        Turing = 6,
        Ampere = 7,
        Ada = 8,
        Hopper = 9,
        Blackwell = 10,
        Unknown = 0xFFFFFFFF
    }

    public enum NvmlPstates
    {
        P0 = 0,
        P1 = 1,
        P2 = 2,
        P3 = 3,
        P4 = 4,
        P5 = 5,
        P6 = 6,
        P7 = 7,
        P8 = 8,
        P12 = 12,
        Unknown = 32
    }

    public enum NvmlClockType : uint
    {
        Graphics = 0,
        Sm = 1,
        Mem = 2,
        Video = 3
    }

    public enum NvmlPcieUtilCounter : uint
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

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NvmlPciInfo
    {
        private unsafe fixed byte busIdLegacy[16];
        public uint Domain;
        public uint Bus;
        public uint Device;
        public uint PciDeviceId;
        public uint PciSubSystemId;
        private unsafe fixed byte _busId[32];

        public readonly unsafe string BusIdLegacy
        {
            get
            {
                fixed (byte* p = busIdLegacy)
                {
                    return ReadFixedString(p, 16);
                }
            }
        }

        public readonly unsafe string BusId
        {
            get
            {
                fixed (byte* p = _busId)
                {
                    return ReadFixedString(p, 32);
                }
            }
        }

        private static unsafe string ReadFixedString(byte* ptr, int maxLength)
        {
            var span = new ReadOnlySpan<byte>(ptr, maxLength);
            var end = span.IndexOf((byte)0);
            return Encoding.UTF8.GetString(end < 0 ? span : span[..end]);
        }
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

    // Static info

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetName")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetName(IntPtr device, Span<byte> name, uint length);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetUUID")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetUUID(IntPtr device, Span<byte> uuid, uint length);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetSerial")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetSerial(IntPtr device, Span<byte> serial, uint length);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetVbiosVersion")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetVbiosVersion(IntPtr device, Span<byte> version, uint length);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetBrand")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetBrand(IntPtr device, out NvmlBrandType type);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetArchitecture")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetArchitecture(IntPtr device, out NvmlDeviceArchitecture arch);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetCudaComputeCapability")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetCudaComputeCapability(IntPtr device, out int major, out int minor);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetNumGpuCores")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetNumGpuCores(IntPtr device, out uint numCores);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetMaxPcieLinkGeneration")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetMaxPcieLinkGeneration(IntPtr device, out uint maxLinkGen);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetMaxPcieLinkWidth")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetMaxPcieLinkWidth(IntPtr device, out uint maxLinkWidth);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetMaxClockInfo")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetMaxClockInfo(IntPtr device, NvmlClockType clockType, out uint clock);

    // System info

    [LibraryImport(NvmlDll, EntryPoint = "nvmlSystemGetDriverVersion")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlSystemGetDriverVersion(Span<byte> version, uint length);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlSystemGetNVMLVersion")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlSystemGetNVMLVersion(Span<byte> version, uint length);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetPciInfo_v3")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetPciInfo(IntPtr device, out NvmlPciInfo pci);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlSystemGetCudaDriverVersion")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlSystemGetCudaDriverVersion(out int cudaDriverVersion);

    // Dynamic additional info

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetEncoderUtilization")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetEncoderUtilization(IntPtr device, out uint utilization, out uint samplingPeriodUs);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetDecoderUtilization")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetDecoderUtilization(IntPtr device, out uint utilization, out uint samplingPeriodUs);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetPerformanceState")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetPerformanceState(IntPtr device, out NvmlPstates pState);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetCurrentClocksThrottleReasons")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetCurrentClocksThrottleReasons(IntPtr device, out ulong clocksThrottleReasons);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetCurrentClocksEventReasons")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetCurrentClocksEventReasons(IntPtr device, out ulong clocksEventReasons);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetCurrPcieLinkGeneration")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetCurrPcieLinkGeneration(IntPtr device, out uint currLinkGen);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetCurrPcieLinkWidth")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetCurrPcieLinkWidth(IntPtr device, out uint currLinkWidth);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetPowerManagementDefaultLimit")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetPowerManagementDefaultLimit(IntPtr device, out uint defaultLimit);

    [LibraryImport(NvmlDll, EntryPoint = "nvmlDeviceGetPowerManagementLimitConstraints")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial NvmlReturn NvmlDeviceGetPowerManagementLimitConstraints(IntPtr device, out uint minLimit, out uint maxLimit);
}
#pragma warning restore CA5392
// ReSharper restore InconsistentNaming
// ReSharper restore IdentifierTypo
// ReSharper restore CommentTypo
