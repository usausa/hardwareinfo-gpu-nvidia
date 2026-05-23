namespace HardwareInfo.Gpu.Nvidia;

using System.Text;

using static HardwareInfo.Gpu.Nvidia.NativeMethods;

public sealed class NvidiaGpuInfo
{
    private readonly nint device;

    private readonly uint[] fanSpeeds;

    //--------------------------------------------------------------------
    // Static properties
    //--------------------------------------------------------------------

    // ReSharper disable CommentTypo
    // ReSharper disable IdentifierTypo

    // GPU Name: ex. "NVIDIA GeForce RTX 4090"
    public string Name { get; }

    // GPU unique UUID: ex. "GPU-xxxxxxxx-xxxx-..."
    public string Uuid { get; }

    // Serial number (empty string for unsupported models)
    public string Serial { get; }

    // VBIOS version
    public string VbiosVersion { get; }

    // GPU brand type
    public GpuBrand Brand { get; }

    // GPU architecture
    public GpuArchitecture Architecture { get; }

    // CUDA Compute Capability major number
    public int ComputeCapabilityMajor { get; }

    // CUDA Compute Capability minor number
    public int ComputeCapabilityMinor { get; }

    // CUDA core count
    public uint CoreCount { get; }

    // PCI Bus ID string: ex. "0000:01:00.0"
    public string PciBusId { get; }

    // Maximum PCIe link generation
    public uint MaxPcieLinkGeneration { get; }

    // Maximum PCIe lane width
    public uint MaxPcieLinkWidth { get; }

    // Maximum Graphics clock in MHz
    public uint MaxClockGraphics { get; }

    // Maximum SM clock in MHz
    public uint MaxClockSm { get; }

    // Maximum Memory clock in MHz
    public uint MaxClockMemory { get; }

    // Maximum Video clock in MHz
    public uint MaxClockVideo { get; }

    // ReSharper restore IdentifierTypo
    // ReSharper disable CommentTypo

    //--------------------------------------------------------------------
    // Fan
    //--------------------------------------------------------------------

    public uint FanCount { get; }

    //--------------------------------------------------------------------
    // Dynamic properties
    //--------------------------------------------------------------------

    // ReSharper disable CommentTypo

    public uint GpuUtilization { get; private set; }
    public uint MemoryUtilization { get; private set; }

    public ulong MemoryTotal { get; private set; }
    public ulong MemoryFree { get; private set; }
    public ulong MemoryUsed { get; private set; }

    public uint PowerUsage { get; private set; }
    public uint PowerLimit { get; private set; }

    // Power limit default value in mW
    public uint PowerLimitDefault { get; private set; }

    // Power limit minimum value in mW
    public uint PowerLimitMin { get; private set; }

    // Power limit maximum value in mW
    public uint PowerLimitMax { get; private set; }

    public uint Temperature { get; private set; }

    // Memory temperature in Celsius (0 for unsupported models)
    public uint MemoryTemperature { get; private set; }

    public uint ClockGraphics { get; private set; }
    public uint ClockSm { get; private set; }
    public uint ClockMemory { get; private set; }
    public uint ClockVideo { get; private set; }

    public uint PcieThroughputTx { get; private set; }
    public uint PcieThroughputRx { get; private set; }

    // NVENC encoder utilization in percent
    public uint EncoderUtilization { get; private set; }

    // NVDEC decoder utilization in percent
    public uint DecoderUtilization { get; private set; }

    // Performance state (P0-P12/Unknown)
    public GpuPerformanceState PerformanceState { get; private set; }

    // Clock throttle reason bit flags
    public ulong ThrottleReasons { get; private set; }

    // Current PCIe link generation
    public uint PcieLinkGeneration { get; private set; }

    // Current PCIe lane width
    public uint PcieLinkWidth { get; private set; }

    // ReSharper restore CommentTypo

    //--------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------

    public NvidiaGpuInfo(nint device)
    {
        this.device = device;

        Span<byte> buffer = stackalloc byte[256];

        Name = ReadString(NvmlDeviceGetName(device, buffer, (uint)buffer.Length), buffer);
        Uuid = ReadString(NvmlDeviceGetUUID(device, buffer, (uint)buffer.Length), buffer);
        Serial = ReadString(NvmlDeviceGetSerial(device, buffer, (uint)buffer.Length), buffer);
        VbiosVersion = ReadString(NvmlDeviceGetVbiosVersion(device, buffer, (uint)buffer.Length), buffer);

        if (NvmlDeviceGetBrand(device, out var brand) == NvmlReturn.Success)
        {
            Brand = (GpuBrand)(int)brand;
        }

        if (NvmlDeviceGetArchitecture(device, out var arch) == NvmlReturn.Success)
        {
            Architecture = (GpuArchitecture)(uint)arch;
        }
        else
        {
            Architecture = GpuArchitecture.Unknown;
        }

        if (NvmlDeviceGetCudaComputeCapability(device, out var major, out var minor) == NvmlReturn.Success)
        {
            ComputeCapabilityMajor = major;
            ComputeCapabilityMinor = minor;
        }

        if (NvmlDeviceGetNumGpuCores(device, out var cores) == NvmlReturn.Success)
        {
            CoreCount = cores;
        }

        PciBusId = NvmlDeviceGetPciInfo(device, out var pci) == NvmlReturn.Success ? pci.BusId : string.Empty;

        if (NvmlDeviceGetMaxPcieLinkGeneration(device, out var maxGen) == NvmlReturn.Success)
        {
            MaxPcieLinkGeneration = maxGen;
        }

        if (NvmlDeviceGetMaxPcieLinkWidth(device, out var maxWidth) == NvmlReturn.Success)
        {
            MaxPcieLinkWidth = maxWidth;
        }

        if (NvmlDeviceGetMaxClockInfo(device, NvmlClockType.Graphics, out var mc0) == NvmlReturn.Success)
        {
            MaxClockGraphics = mc0;
        }

        if (NvmlDeviceGetMaxClockInfo(device, NvmlClockType.Sm, out var mc1) == NvmlReturn.Success)
        {
            MaxClockSm = mc1;
        }

        if (NvmlDeviceGetMaxClockInfo(device, NvmlClockType.Mem, out var mc2) == NvmlReturn.Success)
        {
            MaxClockMemory = mc2;
        }

        if (NvmlDeviceGetMaxClockInfo(device, NvmlClockType.Video, out var mc3) == NvmlReturn.Success)
        {
            MaxClockVideo = mc3;
        }

        if (NvmlDeviceGetNumFans(device, out var fanCount) == NvmlReturn.Success)
        {
            FanCount = fanCount;
        }

        fanSpeeds = new uint[FanCount];
    }

    //--------------------------------------------------------------------
    // Fan
    //--------------------------------------------------------------------

    public uint GetFanSpeed(int index) => index >= 0 && index < FanCount ? fanSpeeds[index] : 0;

    //--------------------------------------------------------------------
    // Update
    //--------------------------------------------------------------------

    public void Update()
    {
        if (NvmlDeviceGetUtilizationRates(device, out var utilization) == NvmlReturn.Success)
        {
            GpuUtilization = utilization.Gpu;
            MemoryUtilization = utilization.Memory;
        }
        else
        {
            GpuUtilization = 0;
            MemoryUtilization = 0;
        }

        if (NvmlDeviceGetMemoryInfo(device, out var memory) == NvmlReturn.Success)
        {
            MemoryTotal = memory.Total;
            MemoryFree = memory.Free;
            MemoryUsed = memory.Used;
        }
        else
        {
            MemoryTotal = 0;
            MemoryFree = 0;
            MemoryUsed = 0;
        }

        if (NvmlDeviceGetPowerUsage(device, out var powerUsage) == NvmlReturn.Success)
        {
            PowerUsage = powerUsage;
        }

        if (NvmlDeviceGetEnforcedPowerLimit(device, out var powerLimit) == NvmlReturn.Success)
        {
            PowerLimit = powerLimit;
        }

        if (NvmlDeviceGetPowerManagementDefaultLimit(device, out var defLimit) == NvmlReturn.Success)
        {
            PowerLimitDefault = defLimit;
        }

        if (NvmlDeviceGetPowerManagementLimitConstraints(device, out var minLimit, out var maxLimit) == NvmlReturn.Success)
        {
            PowerLimitMin = minLimit;
            PowerLimitMax = maxLimit;
        }

        if (NvmlDeviceGetTemperature(device, NvmlTemperatureSensors.Gpu, out var temperature) == NvmlReturn.Success)
        {
            Temperature = temperature;
        }

        if (NvmlDeviceGetTemperature(device, NvmlTemperatureSensors.Memory, out var memTemp) == NvmlReturn.Success)
        {
            MemoryTemperature = memTemp;
        }

        for (var i = 0u; i < FanCount; i++)
        {
            if (NvmlDeviceGetFanSpeed(device, i, out var fanSpeed) == NvmlReturn.Success)
            {
                fanSpeeds[i] = fanSpeed;
            }
        }

        if (NvmlDeviceGetClockInfo(device, NvmlClockType.Graphics, out var clock0) == NvmlReturn.Success)
        {
            ClockGraphics = clock0;
        }

        if (NvmlDeviceGetClockInfo(device, NvmlClockType.Sm, out var clock1) == NvmlReturn.Success)
        {
            ClockSm = clock1;
        }

        if (NvmlDeviceGetClockInfo(device, NvmlClockType.Mem, out var clock2) == NvmlReturn.Success)
        {
            ClockMemory = clock2;
        }

        if (NvmlDeviceGetClockInfo(device, NvmlClockType.Video, out var clock3) == NvmlReturn.Success)
        {
            ClockVideo = clock3;
        }

        if (NvmlDeviceGetPcieThroughput(device, NvmlPcieUtilCounter.TxBytes, out var txBytes) == NvmlReturn.Success)
        {
            PcieThroughputTx = txBytes;
        }

        if (NvmlDeviceGetPcieThroughput(device, NvmlPcieUtilCounter.RxBytes, out var rxBytes) == NvmlReturn.Success)
        {
            PcieThroughputRx = rxBytes;
        }

        if (NvmlDeviceGetEncoderUtilization(device, out var encUtil, out _) == NvmlReturn.Success)
        {
            EncoderUtilization = encUtil;
        }

        if (NvmlDeviceGetDecoderUtilization(device, out var decUtil, out _) == NvmlReturn.Success)
        {
            DecoderUtilization = decUtil;
        }

        if (NvmlDeviceGetPerformanceState(device, out var pState) == NvmlReturn.Success)
        {
            PerformanceState = (GpuPerformanceState)(int)pState;
        }
        else
        {
            PerformanceState = GpuPerformanceState.Unknown;
        }

        if (NvmlDeviceGetCurrentClocksThrottleReasons(device, out var throttle) == NvmlReturn.Success)
        {
            ThrottleReasons = throttle;
        }

        if (NvmlDeviceGetCurrPcieLinkGeneration(device, out var linkGen) == NvmlReturn.Success)
        {
            PcieLinkGeneration = linkGen;
        }

        if (NvmlDeviceGetCurrPcieLinkWidth(device, out var linkWidth) == NvmlReturn.Success)
        {
            PcieLinkWidth = linkWidth;
        }
    }

    //--------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------

    private static string ReadString(NvmlReturn ret, ReadOnlySpan<byte> buffer)
    {
        if (ret != NvmlReturn.Success)
        {
            return string.Empty;
        }

        var end = buffer.IndexOf((byte)0);
        return Encoding.UTF8.GetString(end < 0 ? buffer : buffer[..end]);
    }
}
