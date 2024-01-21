using TimeServer.Application.Interfaces;
using TimeServer.DataAccess;
using TimeServer.DataAccess.Entities;

namespace TimeServer.Infrastructure;

public sealed class AuditableTimeProvider : ITimeProvider
{
    private readonly ITimeProvider _timeProvider;
    private readonly ApplicationDbContext _dbContext;

    public AuditableTimeProvider(TimeProvider timeService, ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _timeProvider = timeService;
    }

    public async ValueTask<DateTimeOffset> GetCurrentTime()
    {
        DateTimeOffset currentTime = await _timeProvider.GetCurrentTime();

        Log logEntity = new()
        {
            ActionName = nameof(GetCurrentTime),
            CreatedAt = currentTime.UtcDateTime
        };

        _dbContext.Add(logEntity);
        await _dbContext.SaveChangesAsync();

        return currentTime;
    }
}
