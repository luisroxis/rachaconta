using RachaConta.Application.DTOs.Request;
using RachaConta.Application.DTOs.Response;
using RachaConta.Core.Interfaces.Repositories;

namespace RachaConta.Application.UseCases;

public class ApproveFriendshipUseCase
{
    private readonly IFriendshipRepository _friendshipRepository;

    public ApproveFriendshipUseCase(IFriendshipRepository friendshipRepository)
    {
        _friendshipRepository = friendshipRepository;
    }

    public async Task<FriendshipResponse?> ExecuteAsync(Guid friendshipId, ApproveFriendshipRequest request, Guid currentUserId)
    {
        var friendship = await _friendshipRepository.GetByIdAsync(friendshipId);

        if (friendship == null)
        {
            throw new InvalidOperationException("Solicitação de amizade não encontrada.");
        }

        // Verify if the current user is the one who received the request
        if (friendship.AmigoId != currentUserId)
        {
            throw new UnauthorizedAccessException("Você não tem permissão para aprovar esta solicitação.");
        }

        if (request.Approved)
        {
            friendship.Approved = true;
            friendship.DataAprovacao = DateTime.UtcNow;
            await _friendshipRepository.UpdateAsync(friendship);

            return new FriendshipResponse
            {
                Id = friendship.Id,
                UserId = friendship.UserId,
                AmigoId = friendship.AmigoId,
                Approved = friendship.Approved,
                Convidado = friendship.Convidado,
                ConvidadoEmail = friendship.ConvidadoEmail,
                DataAprovacao = friendship.DataAprovacao
            };
        }
        else
        {
            // Rejecting means deleting the request
            await _friendshipRepository.DeleteAsync(friendship);
            return null;
        }
    }
}
