using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Ambev.DeveloperEvaluation.Domain.Events.Users.Handlers;

public class UserCreatedEventHandler :
    INotificationHandler<UserRegisteredEvent>,
    INotificationHandler<UserDeletedEvent>,
    INotificationHandler<UserRetrievedEvent>
{
    public UserCreatedEventHandler(IConfiguration config)
    {
        Log.Logger = new LoggerConfiguration()
          .Enrich.FromLogContext()
          .WriteTo.Seq(config["SERILOG_SEQ_URL"]!)
          .CreateLogger();
    }

    public Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        Log.Logger.Information("{User}", notification);
        return Task.CompletedTask;
    }

    public Task Handle(UserRetrievedEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("{UserId}", notification);
        return Task.CompletedTask;
    }

    public Task Handle(UserDeletedEvent notification, CancellationToken cancellationToken)
    {
        Log.Information("{UserId}", notification);
        return Task.CompletedTask;
    }
}
