using DistributedSystemAPI.Abstractions;
using DistributedSystemAPI.Models.Cfg;
using Microsoft.Extensions.Options;
using System.Text;
namespace DistributedSystemAPI.Services;

internal class PayloadManager : IPayloadManager
{
    private readonly ISnapshotManager _snapshotManager;

    private readonly string _directoryPath;
    private readonly string _payloadFilePath;
    private readonly int _snapshotTimeInterval;
    private DateTime _lastSnapshotTime;

    public PayloadManager(IOptions<FileManagmentOptions> options, ISnapshotManager snapshotManager)
    {
        _directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, options.Value.Directory);
        _payloadFilePath = Path.Combine(_directoryPath, options.Value.PayloadFileName);
        _snapshotTimeInterval = options.Value.SnapshotTimeInterval;
        _snapshotManager = snapshotManager;
    }

    public async Task CreateFileAsync(CancellationToken cancellationToken)
    {
        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        if (!File.Exists(_payloadFilePath))
            using (StreamWriter sw = new(_payloadFilePath))
            {
                await sw.WriteAsync("Initial text.");
            }
    }

    public async Task<string> ReadPayloadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_payloadFilePath))
            await CreateFileAsync(cancellationToken);

        using StreamReader sr = new(_payloadFilePath);
        return await sr.ReadToEndAsync(cancellationToken);
    }

    public async Task RewritePayloadAsync(string content, CancellationToken cancellationToken)
    {
        if (!File.Exists(_payloadFilePath))
            await CreateFileAsync(cancellationToken);

        using StreamWriter payloadWriter = new(_payloadFilePath);
        await payloadWriter.WriteAsync(new StringBuilder(content), cancellationToken);

        if(DateTime.Now - _lastSnapshotTime >= TimeSpan.FromMinutes(_snapshotTimeInterval))
        {
            await _snapshotManager.TakeSnapshotAsync(content, cancellationToken);
            _lastSnapshotTime = DateTime.Now;
        }
    }
}
