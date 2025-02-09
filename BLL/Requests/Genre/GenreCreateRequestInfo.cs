namespace BusinessLogic.Requests.Genre;

public class GenreCreateRequestInfo
{
    public string Name { get; set; }

    public Guid? ParentGenreId { get; set; }
}
