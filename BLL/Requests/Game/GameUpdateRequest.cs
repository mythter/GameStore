using BusinessLogic.Dto.Game;

namespace BusinessLogic.Requests.Game;

public class GameUpdateRequest
{
    public GameDto Game { get; set; }

    public IEnumerable<Guid> Genres { get; set; }

    public IEnumerable<Guid> Platforms { get; set; }
}
