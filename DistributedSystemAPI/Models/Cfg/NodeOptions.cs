namespace DistributedSystemAPI.Models.Cfg;

internal record NodeOptions
{
    public bool IsMaster { get; init; }
    public List<string>? Followers { get; init; } 
}
