namespace HardwareInfo.Gpu.Nvidia;

using static HardwareInfo.Gpu.Nvidia.NativeMethods;

public sealed class NvidiaGpuInfo
{
    private readonly nint device;

    private readonly uint[] fanSpeeds;

    public uint FanCount { get; }

    public uint GpuUtilization { get; private set; }
    public uint MemoryUtilization { get; private set; }

    public ulong MemoryTotal { get; private set; }
    public ulong MemoryFree { get; private set; }
    public ulong MemoryUsed { get; private set; }

    public uint PowerUsage { get; private set; }
    public uint PowerLimit { get; private set; }

    public uint Temperature { get; private set; }

    public uint ClockGraphics { get; private set; }
    public uint ClockSm { get; private set; }
    public uint ClockMemory { get; private set; }
    public uint ClockVideo { get; private set; }

    public NvidiaGpuInfo(nint device)
    {
        this.device = device;

        FanCount = NvmlDeviceGetNumFans(device, out var fanCount) == NvmlReturn.Success ? fanCount : 0;
        fanSpeeds = new uint[FanCount];
    }

    public uint GetFanSpeed(int index) =>
        index >= 0 && index < FanCount ? fanSpeeds[index] : 0;

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

        PowerUsage = NvmlDeviceGetPowerUsage(device, out var powerUsage) == NvmlReturn.Success ? powerUsage : 0;
        PowerLimit = NvmlDeviceGetEnforcedPowerLimit(device, out var powerLimit) == NvmlReturn.Success ? powerLimit : 0;

        Temperature = NvmlDeviceGetTemperature(device, NvmlTemperatureSensors.NVML_TEMPERATURE_GPU, out var temperature) == NvmlReturn.Success ? temperature : 0;

        for (var i = 0u; i < FanCount; i++)
        {
            fanSpeeds[i] = NvmlDeviceGetFanSpeed(device, i, out var fanSpeed) == NvmlReturn.Success ? fanSpeed : 0;
        }

        ClockGraphics = NvmlDeviceGetClockInfo(device, NvmlClockType.NVML_CLOCK_GRAPHICS, out var clock0) == NvmlReturn.Success ? clock0 : 0;
        ClockSm = NvmlDeviceGetClockInfo(device, NvmlClockType.NVML_CLOCK_SM, out var clock1) == NvmlReturn.Success ? clock1 : 0;
        ClockMemory = NvmlDeviceGetClockInfo(device, NvmlClockType.NVML_CLOCK_MEM, out var clock2) == NvmlReturn.Success ? clock2 : 0;
        ClockVideo = NvmlDeviceGetClockInfo(device, NvmlClockType.NVML_CLOCK_VIDEO, out var clock3) == NvmlReturn.Success ? clock3 : 0;

        // TODO other values
    }
}
