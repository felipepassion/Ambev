using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events.Handlers;

public class SaleCreatedEventHandler : INotificationHandler<SaleCreatedEvent>
{
    public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
