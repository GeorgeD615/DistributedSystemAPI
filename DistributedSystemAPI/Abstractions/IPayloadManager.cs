namespace DistributedSystemAPI.Abstractions;

internal interface IPayloadManager
{
    Task RewritePayloadAsync(string content, CancellationToken cancellationToken);

    Task<string> ReadPayloadAsync(CancellationToken cancellationToken);
}
