namespace DistributedSystemAPI.Abstractions;

internal interface ISnapshotManager
{
    Task TakeSnapshotAsync(string content, CancellationToken cancellationToken);
}
