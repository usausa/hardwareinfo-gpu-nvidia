using HardwareInfo.Gpu.Nvidia;

NvidiaGpu.Initialize();

try
{
    var gpus = NvidiaGpu.GetInformation();

    while (true)
    {
        foreach (var gpu in gpus)
        {
            gpu.Update();
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} | {gpu.GpuUtilization}% {gpu.MemoryUtilization}% {gpu.Temperature}");
        }

        Thread.Sleep(1000);
    }
}
finally
{
    NvidiaGpu.Shutdown();
}
