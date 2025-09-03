namespace HardwareInfo.Gpu.Nvidia;

public sealed class NvidiaGpu
{
    public static void Initialize()
    {
        // TODO
    }

    public static void Shutdown()
    {
        // TODO
    }

    public static IReadOnlyList<NvidiaGpuInfo> GetInformation()
    {
        // TODO 1st Update
        // ReSharper disable once CollectionNeverUpdated.Local
        var list = new List<NvidiaGpuInfo>();

        var gpu = new NvidiaGpuInfo();
        gpu.Update();

        list.Add(gpu);

        return list;
    }
}
