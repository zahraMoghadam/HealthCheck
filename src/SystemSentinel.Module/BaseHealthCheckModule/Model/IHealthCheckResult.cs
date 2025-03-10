using SystemSentinel.BaseHealthCheck.Module.Enum;

namespace SystemSentinel.BaseHealthCheck.Module.Model
{
    public interface IHealthCheckResult
    {
        CheckStatus CheckStatus { get; }
        string Description { get; }
        IReadOnlyDictionary<string, object> Data { get; }
    }
}
