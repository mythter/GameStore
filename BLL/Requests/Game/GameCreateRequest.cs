namespace BusinessLogic.Requests.Game;

public class GameCreateRequest
{
    public GameCreateRequestInfo Game { get; set; }

    public IEnumerable<Guid> Genres { get; set; }

    public IEnumerable<Guid> Platforms { get; set; }
}
