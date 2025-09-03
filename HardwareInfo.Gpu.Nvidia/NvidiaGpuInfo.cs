namespace HardwareInfo.Gpu.Nvidia;

public sealed class NvidiaGpuInfo
{
    public bool LastUpdate { get; private set; }

    public bool Update()
    {
        // TODO
        var ret = true;

        LastUpdate = ret;
        return true;
    }
}
