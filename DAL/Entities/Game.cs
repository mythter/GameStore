using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class Game
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Key { get; set; } = null!;

    public string? Description { get; set; }
}
