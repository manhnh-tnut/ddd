using PRO.Domain.Entities.Users;
using PRO.Domain.Entities.Users.Events;
using MediatR;

namespace PRO.Api.Features.User.Events;

public class RegisteredEventHandler
                : INotificationHandler<OnRegisteredEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly ILoggerFactory _logger;

    public RegisteredEventHandler(
        IUserRepository userRepository, ILoggerFactory logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(OnRegisteredEvent notification, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(_=>_.Id.Equals(notification.User.Id));
        _logger.CreateLogger<RegisteredEventHandler>()
            .LogTrace("Event with Id: {EventId} has been registered for user {user} ({Id})",
                notification.EventId, user?.UserName, user?.Id);
    }
}
