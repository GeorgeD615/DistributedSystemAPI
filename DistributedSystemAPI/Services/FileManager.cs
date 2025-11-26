using DistributedSystemAPI.Abstractions;
using DistributedSystemAPI.Models.Cfg;
using Microsoft.Extensions.Options;
using System.Text;
namespace DistributedSystemAPI.Services;

internal class FileManager : IFileManager
{
    private readonly string _directoryPath;
    private readonly string _filePath;

    public FileManager(IOptions<FileLocation> fileLocation)
    {
        _directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileLocation.Value.Directory);
        _filePath = Path.Combine(_directoryPath, fileLocation.Value.FileName);
    }

    public async Task CreateFile(CancellationToken cancellationToken)
    {
        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        if (!File.Exists(_filePath))
            using (StreamWriter sw = new(_filePath))
            {
                await sw.WriteAsync("Initial text.");
            }
    }

    public async Task<string> ReadFromFile(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
            await CreateFile(cancellationToken);

        using StreamReader sr = new(_filePath);
        return await sr.ReadToEndAsync(cancellationToken);
    }

    public async Task WriteIntoFile(string content, CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
            await CreateFile(cancellationToken);

        using StreamWriter sw = new(_filePath);
        await sw.WriteAsync(new StringBuilder(content), cancellationToken);
    }
}
