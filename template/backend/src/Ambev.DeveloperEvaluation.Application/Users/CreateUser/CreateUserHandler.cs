// CreateUserHandler.cs
using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Events.Users;

namespace Ambev.DeveloperEvaluation.Application.Users.CreateUser
{
    /// <summary>
    /// Handler for processing CreateUserCommand requests.
    /// Validates the command, creates a new user, persists it, and publishes a UserCreatedEvent.
    /// </summary>
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMediator _mediator;

        public CreateUserHandler(IUserRepository userRepository, IMapper mapper, IPasswordHasher passwordHasher, IMediator mediator)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _mediator = mediator;
        }

        public async Task<CreateUserResult> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var validator = new CreateUserCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existingUser = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
            if (existingUser != null)
                throw new InvalidOperationException($"User with email {command.Email} already exists");

            var user = _mapper.Map<User>(command);
            user.Password = _passwordHasher.HashPassword(command.Password);

            var createdUser = await _userRepository.CreateAsync(user, cancellationToken);
            var result = _mapper.Map<CreateUserResult>(createdUser);

            // Publish UserCreatedEvent after successful creation
            await _mediator.Publish(new UserRegisteredEvent(
                new User
                {
                    Id = createdUser.Id,
                    Email = createdUser.Email,
                    Username =
                createdUser.Username
                }), cancellationToken);

            return result;
        }
    }
}
