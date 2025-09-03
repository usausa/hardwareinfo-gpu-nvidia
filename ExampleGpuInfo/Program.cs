using HardwareInfo.Gpu.Nvidia;

foreach (var gpu in NvidiaGpu.GetInformation())
{
    // TODO
    Console.WriteLine($"{gpu}");
}
