namespace GamePlatform.Common.Settings;

public class RabbitMqSettings
{
    public required string Host { get; init; }
    public int Port { get; init; }
    public string ConnectionString => $"amqp://{Host}:{Port}";
}