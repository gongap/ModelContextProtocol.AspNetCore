using System.Diagnostics;

namespace ModelContextProtocol.AspNetCore;

public class TimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateTimeOffset Now => DateTimeOffset.Now;

    public virtual long GetTimestamp() => Stopwatch.GetTimestamp();

    public static TimeProvider System { get; } = (TimeProvider) new TimeProvider.SystemTimeProvider();

    private sealed class SystemTimeProvider : TimeProvider
    {
        internal SystemTimeProvider()
        {
        }
    }
}