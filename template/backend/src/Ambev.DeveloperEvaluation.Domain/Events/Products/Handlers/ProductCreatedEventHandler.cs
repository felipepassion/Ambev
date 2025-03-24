using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Ambev.DeveloperEvaluation.Domain.Events.Products.Handlers;

public class ProductCreatedEventHandler :
    INotificationHandler<ProductCreatedEvent>,
    INotificationHandler<ProductDeletedEvent>,
    INotificationHandler<ProductRetrievedEvent>
{
    public ProductCreatedEventHandler(IConfiguration config)
    {
        Log.Logger = new LoggerConfiguration()
           .Enrich.FromLogContext()
           .WriteTo.Seq(config["SERILOG_SEQ_URL"]!)
           .CreateLogger();
    }

    public Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("{Product}", notification);
        return Task.CompletedTask;
    }

    public Task Handle(ProductRetrievedEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("{ProductId}", notification);
        return Task.CompletedTask;
    }

    public Task Handle(ProductDeletedEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("{ProductId}", notification);
        return Task.CompletedTask;
    }
}
