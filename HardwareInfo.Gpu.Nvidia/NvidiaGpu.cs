namespace HardwareInfo.Gpu.Nvidia;

using System.Text;

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

    public static string GetDriverVersion()
    {
        Span<byte> buffer = stackalloc byte[80];
        return NvmlSystemGetDriverVersion(buffer, (uint)buffer.Length) == NvmlReturn.Success ? GetString(buffer) : string.Empty;
    }

    public static string GetNvmlVersion()
    {
        Span<byte> buffer = stackalloc byte[80];
        return NvmlSystemGetNVMLVersion(buffer, (uint)buffer.Length) == NvmlReturn.Success ? GetString(buffer) : string.Empty;
    }

    public static int GetCudaDriverVersion()
    {
        return NvmlSystemGetCudaDriverVersion(out var version) == NvmlReturn.Success ? version : 0;
    }

    private static string GetString(ReadOnlySpan<byte> buffer)
    {
        var end = buffer.IndexOf((byte)0);
        return Encoding.UTF8.GetString(end < 0 ? buffer : buffer[..end]);
    }
}
