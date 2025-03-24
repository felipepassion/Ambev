using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Ambev.DeveloperEvaluation.Domain.Events.Sales.Handlers;

public class SaleCreatedEventHandler :
    INotificationHandler<SaleCreatedEvent>,
    INotificationHandler<SaleDeletedEvent>,
    INotificationHandler<SaleItemCreatedEvent>,
    INotificationHandler<SaleRetrievedEvent>
{
    public SaleCreatedEventHandler(IConfiguration config)
    {
        Log.Logger = new LoggerConfiguration()
          .Enrich.FromLogContext()
          .WriteTo.Seq(config["SERILOG_SEQ_URL"]!)
          .CreateLogger();
    }

    public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("{SaleId}", notification);
        return Task.CompletedTask;
    }

    public Task Handle(SaleRetrievedEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("{SaleId}", notification);
        return Task.CompletedTask;
    }

    public Task Handle(SaleItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("item {SaleItemId}", notification);
        return Task.CompletedTask;
    }

    public Task Handle(SaleDeletedEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("{SaleId}", notification);
        return Task.CompletedTask;
    }
}
