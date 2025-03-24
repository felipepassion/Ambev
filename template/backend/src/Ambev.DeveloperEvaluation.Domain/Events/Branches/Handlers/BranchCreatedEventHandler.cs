using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Ambev.DeveloperEvaluation.Domain.Events.Branches.Handlers;

public class BranchCreatedEventHandler :
    INotificationHandler<BranchCreatedEvent>,
    INotificationHandler<BranchDeletedEvent>,
    INotificationHandler<BranchRetrievedEvent>
{
    public BranchCreatedEventHandler(IConfiguration config)
    {
        Log.Logger = new LoggerConfiguration()
          .Enrich.FromLogContext()
          .WriteTo.Seq(config["SERILOG_SEQ_URL"]!)
          .CreateLogger();
    }

    public Task Handle(BranchCreatedEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("{BranchId}", notification); 
        return Task.CompletedTask;
    }

    public Task Handle(BranchRetrievedEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("{BranchId}", notification);
        return Task.CompletedTask;
    }

    public Task Handle(BranchDeletedEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("{BranchId}", notification);
        return Task.CompletedTask;
    }
}
