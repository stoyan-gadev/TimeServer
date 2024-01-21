using System.ComponentModel.DataAnnotations.Schema;

namespace TimeServer.DataAccess.Entities;

public sealed class Log
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public string ActionName { get; init; } = string.Empty;

    public string? Description { get; init; }
}
