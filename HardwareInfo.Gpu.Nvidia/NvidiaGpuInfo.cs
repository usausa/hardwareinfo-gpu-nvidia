namespace HardwareInfo.Gpu.Nvidia;

using static HardwareInfo.Gpu.Nvidia.NativeMethods;

public sealed class NvidiaGpuInfo
{
    private readonly nint device;

    public uint Temperature { get; private set; }

    public uint GpuUtilization { get; private set; }

    public uint MemoryUtilization { get; private set; }

    public NvidiaGpuInfo(nint device)
    {
        this.device = device;
    }

    public void Update()
    {
        Temperature = NvmlDeviceGetTemperature(device, NvmlTemperatureSensors.NVML_TEMPERATURE_GPU, out var temperature) == NvmlReturn.Success ? temperature : 0;

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

        // TODO other values
    }
}
