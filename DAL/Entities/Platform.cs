using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class Platform
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Type { get; set; }
}
