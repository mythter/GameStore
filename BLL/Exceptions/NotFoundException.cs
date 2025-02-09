using System.Net;

namespace BusinessLogic.Exceptions;

public class NotFoundException(string message) : HttpException(message, HttpStatusCode.NotFound)
{
}
