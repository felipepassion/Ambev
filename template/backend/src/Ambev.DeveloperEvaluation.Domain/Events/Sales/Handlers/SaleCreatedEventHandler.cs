using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events.Sales.Handlers;

public class SaleCreatedEventHandler : INotificationHandler<SaleCreatedEvent>
{
    public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
