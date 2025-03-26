using System.ComponentModel.DataAnnotations;

namespace ConnectionLogger.Data.Models;

public record IpAddress
{
    [Key]
    public long Id { get; init; }

    [MaxLength(45)]
    [Required]
    public required string Address { get; init; }

    [MaxLength(15)]
    [Required]
    public required string Protocol { get; init; }
}
