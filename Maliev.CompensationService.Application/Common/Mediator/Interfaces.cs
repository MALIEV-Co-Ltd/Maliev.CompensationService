namespace Maliev.CompensationService.Application.Common.Mediator;

/// <summary>
/// Marker interface to represent a request with a response.
/// </summary>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IRequest<out TResponse> { }

/// <summary>
/// Marker interface to represent a request with a void response.
/// </summary>
public interface IRequest : IRequest<Unit> { }

/// <summary>
/// Represents a void type, since <see cref="System.Void"/> is not a valid return type in C#.
/// </summary>
public struct Unit
{
    /// <summary>
    /// Default and only value of the <see cref="Unit"/> type.
    /// </summary>
    public static readonly Unit Value = new Unit();

    /// <summary>
    /// Task from the default value of the <see cref="Unit"/> type.
    /// </summary>
    public static readonly Task<Unit> Task = System.Threading.Tasks.Task.FromResult(Value);
}

/// <summary>
/// Defines a handler for a request.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
/// <typeparam name="TResponse">The type of response from the handler</typeparam>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Defines a handler for a request with a void response.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Unit>
    where TRequest : IRequest<Unit>
{
}

/// <summary>
/// Defines a mediator to encapsulate request/response interaction.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Asynchronously send a request to a single handler
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="request">Request object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task that represents the send operation. The task result contains the handler response</returns>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously send a request to a single handler with no response
    /// </summary>
    /// <param name="request">Request object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task that represents the send operation</returns>
    Task Send(IRequest request, CancellationToken cancellationToken = default);
}
