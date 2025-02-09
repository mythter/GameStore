namespace BusinessLogic.Dto.Genre;

public class GenreDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public Guid? ParentGenreId { get; set; }
}
