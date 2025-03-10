using SystemSentinel.BaseHealthCheck.Module.Enum;
using SystemSentinel.BaseHealthCheck.Module.Extentions;
using SystemSentinel.Common.Utilities;

namespace SystemSentinel.BaseHealthCheck.Module
{
    public class HealthCheckGroup
    {
            private CheckStatus _partialSuccessStatus;

            public HealthCheckGroup(string groupName, CheckStatus partialSuccessStatus)
            {
                Guard.ArgumentNotNull(nameof(groupName), groupName);

                GroupName = groupName;
                PartiallyHealthyStatus = partialSuccessStatus;
            }

            public IReadOnlyList<CachedHealthCheck> Checks => ChecksInternal.AsReadOnly();

            internal List<CachedHealthCheck> ChecksInternal { get; } = new List<CachedHealthCheck>();

            public string GroupName { get; }

            public CheckStatus PartiallyHealthyStatus
            {
                get => _partialSuccessStatus;
                internal set
                {
                    Guard.ArgumentValid(value != CheckStatus.Unknown, nameof(value), "Check status 'Unknown' is not valid for partial success.");

                    _partialSuccessStatus = value;
                }
            }
    }
}
