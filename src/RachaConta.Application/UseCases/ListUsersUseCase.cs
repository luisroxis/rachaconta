using RachaConta.Application.DTOs.Response;
using RachaConta.Application.Interfaces;

namespace RachaConta.Application.UseCases;

public class ListUsersUseCase
{
    private readonly IUserRepository _userRepository;

    public ListUsersUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserResponse>> ExecuteAsync(Guid currentUserId)
    {
        var users = await _userRepository.ListAllExceptAsync(currentUserId);

        return users.Select(u => new UserResponse
        {
            Id = u.Id,
            Name = u.Name,
            UserName = u.UserName,
            Email = u.Email,
            Avatar = u.Avatar
        }).ToList();
    }
}
