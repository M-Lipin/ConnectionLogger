using System.ComponentModel.DataAnnotations;

namespace ConnectionLogger.Data.Models;

public record User
{
    [Key]
    public long Id { get; init; }

    [MaxLength(45)]
    public required string LastName { get; init; }

    [MaxLength(45)]
    public required string FirstName { get; init; }
}
