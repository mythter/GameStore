using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class GameGenre
{
    [Required]
    public Guid GameId { get; set; }

    public Game? Game { get; set; }

    [Required]
    public Guid GenreId { get; set; }

    public Genre? Genre { get; set; }
}
