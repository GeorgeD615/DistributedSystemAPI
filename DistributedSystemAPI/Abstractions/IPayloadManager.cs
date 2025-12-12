using DistributedSystemAPI.Models;

namespace DistributedSystemAPI.Abstractions;

internal interface IPayloadManager
{
    Task RewritePayloadAsync(ReplaceRequestModel model, CancellationToken cancellationToken);

    Task<string> ReadPayloadAsync(CancellationToken cancellationToken);

    string ClockTableJson { get; }
}
