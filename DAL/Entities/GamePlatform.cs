using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class GamePlatform
{
    [Required]
    public Guid GameId { get; set; }

    public Game? Game { get; set; }

    [Required]
    public Guid PlatformId { get; set; }

    public Platform? Platform { get; set; }
}
