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
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} | " +
                              $"{gpu.GpuUtilization}% {gpu.MemoryUtilization}% " +
                              $"{gpu.Temperature} {gpu.MemoryTotal / 1024 / 1024}MB/{gpu.MemoryFree / 1024 / 1024}MB/{gpu.MemoryUsed / 1024 / 1024}MB " +
                              $"{gpu.PowerUsage / 1000}W/{gpu.PowerLimit / 1024}W " +
                              $"{gpu.GetFanSpeed(0)}%/{gpu.GetFanSpeed(1)}%/{gpu.GetFanSpeed(2)}% " +
                              $"{gpu.ClockGraphics}MHz/{gpu.ClockSm}MHz/{gpu.ClockMemory}MHz/{gpu.ClockVideo}MHz " +
                              $"{gpu.PcieThroughputTx}/{gpu.PcieThroughputRx}");
        }

        Thread.Sleep(1000);
    }
}
finally
{
    NvidiaGpu.Shutdown();
}
