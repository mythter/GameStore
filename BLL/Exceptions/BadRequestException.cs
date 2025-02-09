using System.Net;

namespace BusinessLogic.Exceptions;

public class BadRequestException(string message) : HttpException(message, HttpStatusCode.BadRequest)
{
}
