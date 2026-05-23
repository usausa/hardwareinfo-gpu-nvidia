namespace HardwareInfo.Gpu.Nvidia;

#pragma warning disable CA1008
#pragma warning disable CA1027
#pragma warning disable CA1028
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
public enum GpuBrand
{
    Unknown = 0,
    Quadro = 1,
    Tesla = 2,
    NVS = 3,
    Grid = 4,
    GeForce = 5,
    Titan = 6,
    NvidiaVApps = 7,
    GeForceRtx = 8,
    Titan2 = 9
}

public enum GpuArchitecture : uint
{
    Kepler = 2,
    Maxwell = 3,
    Pascal = 4,
    Volta = 5,
    Turing = 6,
    Ampere = 7,
    Ada = 9,
    Hopper = 10,
    Unknown = 0xFFFFFFFF
}

public enum GpuPerformanceState
{
    P0 = 0,
    P1 = 1,
    P2 = 2,
    P3 = 3,
    P4 = 4,
    P5 = 5,
    P6 = 6,
    P7 = 7,
    P8 = 8,
    P12 = 12,
    Unknown = 32
}
// ReSharper restore IdentifierTypo
// ReSharper restore InconsistentNaming
#pragma warning restore CA1028
#pragma warning restore CA1027
#pragma warning restore CA1008
