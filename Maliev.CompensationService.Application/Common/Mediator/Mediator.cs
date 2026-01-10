using Microsoft.Extensions.DependencyInjection;

namespace Maliev.CompensationService.Application.Common.Mediator;

/// <summary>
/// Default implementation of <see cref="IMediator"/>.
/// </summary>
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, System.Reflection.MethodInfo> _handlerMethods = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider to resolve handlers.</param>
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
        {
            throw new InvalidOperationException($"Handler not found for request of type {requestType.Name}.");
        }

        var method = _handlerMethods.GetOrAdd(handlerType, t => t.GetMethod("Handle")
            ?? throw new InvalidOperationException($"Handle method not found on handler {t.Name}"));

        var task = (Task<TResponse>)method.Invoke(handler, new object[] { request, cancellationToken })!;
        return await task;
    }

    /// <inheritdoc/>
    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        await Send<Unit>(request, cancellationToken);
    }
}