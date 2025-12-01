using DistributedSystemAPI.Abstractions;
using DistributedSystemAPI.Models.Cfg;
using Microsoft.Extensions.Options;
using System.Text;

namespace DistributedSystemAPI.Services;

internal class SnapshotManager: ISnapshotManager
{
    private readonly string _directoryPath;
    private readonly string _snapshotFilePath;

    public SnapshotManager(IOptions<FileManagmentOptions> options)
    {
        _directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, options.Value.Directory);
        _snapshotFilePath = Path.Combine(_directoryPath, options.Value.SnapshotFileName);
    }

    private async Task CreateFileAsync(CancellationToken cancellationToken)
    {
        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        if (!File.Exists(_snapshotFilePath))
            using (StreamWriter sw = new(_snapshotFilePath))
            {
                await sw.WriteAsync("Snapshoots:\n");
            }
    }

    public async Task TakeSnapshotAsync(string content, CancellationToken cancellationToken)
    {
        if (!File.Exists(_snapshotFilePath))
            await CreateFileAsync(cancellationToken);

        using StreamWriter snapShotWriter = new(_snapshotFilePath, true);
        await snapShotWriter.WriteLineAsync(new StringBuilder(content), cancellationToken);
    }
}
