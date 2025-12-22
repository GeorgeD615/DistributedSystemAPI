namespace DistributedSystemAPI.Models;

internal record ReplaceRequestModel
{
    public required string Source { get; init; }

    public required int ID { get; init; }

    public required string Payload { get; init; }
}