using Grpc.Core;
using TimeServer.Application.Interfaces;

namespace TimeServer.Services;

public sealed class TimeService : TimeServer.TimeService.TimeServiceBase
{
    private readonly ITimeProvider _timeProvider;

    public TimeService(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public override async Task<TimeResponse> GetCurrentTime(TimeRequest request, ServerCallContext context)
    {
        DateTimeOffset currentTime = await _timeProvider.GetCurrentTime();

        return new TimeResponse
        {
            Message = $"UTC time: {currentTime.UtcDateTime}; Local time: {currentTime.DateTime}"
        };
    }
}
