namespace GamePlatform.Common.Settings;


public class MongoDbSettings
{
    public required string Host { get; init; }
    public int Port { get; init; }
    public string ConnectionString => $"mongodb://{Host}:{Port}";

    public required string DatabaseName { get; init; }
   // public string CollectionName { get; init; } = string.Empty;
}