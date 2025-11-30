namespace HardwareInfo.Gpu.Nvidia;

using static HardwareInfo.Gpu.Nvidia.NativeMethods;

public static class NvidiaGpu
{
#if NET9_0_OR_GREATER
    private static readonly Lock Sync = new();
#else
    private static readonly object Sync = new();
#endif

    // ReSharper disable once MemberCanBePrivate.Global
    public static bool IsAvailable { get; private set; }

    public static void Initialize()
    {
        lock (Sync)
        {
            if (IsAvailable)
            {
                return;
            }

            var ret = NvmlInit();

            IsAvailable = ret == NvmlReturn.Success;
        }
    }

    public static void Shutdown()
    {
        lock (Sync)
        {
            if (!IsAvailable)
            {
                return;
            }

            NvmlShutdown();

            IsAvailable = false;
        }
    }

    public static IReadOnlyList<NvidiaGpuInfo> GetInformation()
    {
        var list = new List<NvidiaGpuInfo>();

        if (NvmlDeviceGetCount(out var deviceCount) == NvmlReturn.Success)
        {
            for (uint i = 0; i < deviceCount; i++)
            {
                if (NvmlDeviceGetHandleByIndex(i, out var device) != NvmlReturn.Success)
                {
                    continue;
                }

                var gpu = new NvidiaGpuInfo(device);
                gpu.Update();

                list.Add(gpu);
            }
        }

        return list;
    }
}
