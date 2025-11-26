namespace DistributedSystemAPI.Models.Cfg;

internal record FileLocation
{
    public required string FileName { get; init; }

    public required string Directory { get; init; }
}
