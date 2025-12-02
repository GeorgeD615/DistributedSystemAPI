namespace DistributedSystemAPI.Models.Cfg;

internal record FileManagmentOptions
{
    public required string PayloadFileName { get; init; }

    public required string SnapshotFileName { get; init; }

    public required string Directory { get; init; }

    public required int SnapshotTimeInterval { get; init; }
}
