namespace Chess.API.Hubs;

public record StatusResponse(StatusCode Code, string Message);

/// <summary>
/// Code that is returned to the client, based on HTML status codes.
/// </summary>
public enum StatusCode
{
    /// <summary>
    /// Successful request.
    /// </summary>
    Success = 200,

    /// <summary>
    /// Request is not valid.
    /// </summary>
    RequestError = 400,

    /// <summary>
    /// Entity was not found.
    /// </summary>
    NotFound = 404,

    /// <summary>
    /// Unexpected Serverside Error.
    /// </summary>
    ServerError = 500,
}
