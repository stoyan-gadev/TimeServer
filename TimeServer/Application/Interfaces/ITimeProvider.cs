namespace TimeServer.Application.Interfaces;

public interface ITimeProvider
{
    ValueTask<DateTimeOffset> GetCurrentTime();
}
