using HardwareInfo.Gpu.Nvidia;

NvidiaGpu.Initialize();

try
{
    // System info
    Console.WriteLine("==== System Info ====");
    Console.WriteLine($"Driver  : {NvidiaGpu.GetDriverVersion()}");
    Console.WriteLine($"NVML    : {NvidiaGpu.GetNvmlVersion()}");
    var cuda = NvidiaGpu.GetCudaDriverVersion();
    Console.WriteLine($"CUDA    : {cuda / 1000}.{(cuda % 1000) / 10}");
    Console.WriteLine();

    var gpus = NvidiaGpu.GetInformation();

    // Static info per GPU
    foreach (var gpu in gpus)
    {
        Console.WriteLine("==== GPU Static Info ====");
        Console.WriteLine($"Name        : {gpu.Name}");
        Console.WriteLine($"UUID        : {gpu.Uuid}");
        Console.WriteLine($"Serial      : {(string.IsNullOrEmpty(gpu.Serial) ? "(N/A)" : gpu.Serial)}");
        Console.WriteLine($"VBIOS       : {gpu.VbiosVersion}");
        Console.WriteLine($"Brand       : {gpu.Brand}");
        Console.WriteLine($"Architecture: {gpu.Architecture}");
        Console.WriteLine($"CUDA CC     : {gpu.ComputeCapabilityMajor}.{gpu.ComputeCapabilityMinor}");
        Console.WriteLine($"Core Count  : {gpu.CoreCount}");
        Console.WriteLine($"PCI Bus ID  : {gpu.PciBusId}");
        Console.WriteLine($"PCIe Max    : Gen{gpu.MaxPcieLinkGeneration} x{gpu.MaxPcieLinkWidth}");
        Console.WriteLine($"Max Clock   : Gfx={gpu.MaxClockGraphics}MHz SM={gpu.MaxClockSm}MHz Mem={gpu.MaxClockMemory}MHz Vid={gpu.MaxClockVideo}MHz");
        Console.WriteLine($"Fan Count   : {gpu.FanCount}");
        Console.WriteLine();
    }

    while (true)
    {
        foreach (var gpu in gpus)
        {
            gpu.Update();

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {gpu.Name}");
            Console.WriteLine($"  Util      : GPU={gpu.GpuUtilization}% Mem={gpu.MemoryUtilization}% Enc={gpu.EncoderUtilization}% Dec={gpu.DecoderUtilization}%");
            Console.WriteLine($"  Memory    : {gpu.MemoryUsed / 1024 / 1024}MB / {gpu.MemoryTotal / 1024 / 1024}MB (Free={gpu.MemoryFree / 1024 / 1024}MB)");
            Console.WriteLine($"  Power     : {gpu.PowerUsage / 1000}W / Enforced={gpu.PowerLimit / 1000}W Default={gpu.PowerLimitDefault / 1000}W Min={gpu.PowerLimitMin / 1000}W Max={gpu.PowerLimitMax / 1000}W");
            Console.WriteLine($"  Temp      : GPU={gpu.Temperature}°C Mem={gpu.MemoryTemperature}°C");
            Console.WriteLine($"  PState    : {gpu.PerformanceState}  Throttle=0x{gpu.ThrottleReasons:X}");
            Console.WriteLine($"  Clock     : Gfx={gpu.ClockGraphics}MHz SM={gpu.ClockSm}MHz Mem={gpu.ClockMemory}MHz Vid={gpu.ClockVideo}MHz");
            Console.WriteLine($"  PCIe      : Gen{gpu.PcieLinkGeneration} x{gpu.PcieLinkWidth}  Tx={gpu.PcieThroughputTx}KB/s Rx={gpu.PcieThroughputRx}KB/s");

            var fans = Enumerable.Range(0, (int)gpu.FanCount).Select(i => $"{gpu.GetFanSpeed(i)}%");
            Console.WriteLine($"  Fan       : {string.Join(" / ", fans)}");

            Console.WriteLine();
        }

        Thread.Sleep(1000);
    }
}
finally
{
    NvidiaGpu.Shutdown();
}
