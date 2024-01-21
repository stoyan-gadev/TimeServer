using TimeServer.Application.Interfaces;

namespace TimeServer.Infrastructure;

public sealed class TimeProvider : ITimeProvider
{
    public ValueTask<DateTimeOffset> GetCurrentTime()
        => ValueTask.FromResult(DateTimeOffset.Now);
}
