using RachaConta.Application.DTOs.Request;
using RachaConta.Application.DTOs.Response;
using RachaConta.Core.Entities;
using RachaConta.Core.Interfaces.Repositories;

namespace RachaConta.Application.UseCases;

public class RequestFriendshipUseCase
{
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IUserRepository _userRepository;

    public RequestFriendshipUseCase(IFriendshipRepository friendshipRepository, IUserRepository userRepository)
    {
        _friendshipRepository = friendshipRepository;
        _userRepository = userRepository;
    }

    public async Task<FriendshipResponse> ExecuteAsync(RequestFriendshipRequest request, Guid currentUserId)
    {
        // Check if friendship already exists
        var existingFriendship = await _friendshipRepository.GetByUsersAsync(currentUserId, request.AmigoId);
        if (existingFriendship != null)
        {
            throw new InvalidOperationException("Já existe uma solicitação de amizade ou amizade entre estes usuários.");
        }

        // Get friend details
        var friend = await _userRepository.GetByIdAsync(request.AmigoId);
        if (friend == null)
        {
            throw new InvalidOperationException("Usuário amigo não encontrado.");
        }

        // Create friendship
        var friendship = new Friendship(
            userId: currentUserId,
            amigoId: request.AmigoId,
            convidado: friend.Name,
            convidadoEmail: friend.Email
        );

        var createdFriendship = await _friendshipRepository.AddAsync(friendship);

        return new FriendshipResponse
        {
            Id = createdFriendship.Id,
            UserId = createdFriendship.UserId,
            AmigoId = createdFriendship.AmigoId,
            Approved = createdFriendship.Approved,
            Convidado = createdFriendship.Convidado,
            ConvidadoEmail = createdFriendship.ConvidadoEmail,
            DataAprovacao = createdFriendship.DataAprovacao
        };
    }
}
