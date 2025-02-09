using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class Genre
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    public Guid? ParentGenreId { get; set; }
}
