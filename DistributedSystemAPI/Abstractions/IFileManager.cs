namespace DistributedSystemAPI.Abstractions
{
    public interface IFileManager
    {
        Task CreateFile(CancellationToken cancellationToken);

        Task WriteIntoFile(string content, CancellationToken cancellationToken);

        Task<string> ReadFromFile(CancellationToken cancellationToken);
    }
}
