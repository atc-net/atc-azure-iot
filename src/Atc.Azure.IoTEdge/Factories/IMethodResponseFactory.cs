namespace Atc.Azure.IoTEdge.Factories;

/// <summary>
/// Represents a factory for creating method response objects that encapsulate
/// the status code and data returned from a method or operation.
/// </summary>
public interface IMethodResponseFactory
{
    /// <summary>
    /// Creates a method response object with the specified status code and data.
    /// </summary>
    /// <param name="statusCode">The HTTP status code associated with the response.</param>
    /// <param name="data">The data or content associated with the response.</param>
    /// <returns>A method response object containing the provided status code and data.</returns>
    MethodResponse Create(
        HttpStatusCode statusCode,
        object data);
}