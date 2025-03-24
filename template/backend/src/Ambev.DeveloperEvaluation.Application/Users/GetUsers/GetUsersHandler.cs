using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.GetUsers
{
    /// <summary>
    /// Handler for processing GetUsersCommand requests.
    /// Retrieves a paginated list of users and maps them to the application DTO.
    /// </summary>
    public class GetUsersHandler : IRequestHandler<GetUsersCommand, List<GetUserResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUsersHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<GetUserResult>> Handle(GetUsersCommand request, CancellationToken cancellationToken)
        {
            // Chama o novo método do repositório que retorna os usuários paginados.
            var users = await _userRepository.GetAllUsersAsync(request.PageNumber, request.PageSize, cancellationToken);

            // Mapeia cada entidade User para o DTO GetUserResult
            var results = users.Select(u => _mapper.Map<GetUserResult>(u)).ToList();

            return results;
        }
    }
}