using System.Net;

namespace BusinessLogic.Exceptions;

public class HttpException(
    string message,
    HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    : Exception(message)
{
    public HttpStatusCode StatusCode => statusCode;
}