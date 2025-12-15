using RachaConta.Application.DTOs.Response;
using RachaConta.Core.Interfaces.Repositories;

namespace RachaConta.Application.UseCases;

public class ListPendingFriendshipsUseCase
{
    private readonly IFriendshipRepository _friendshipRepository;

    public ListPendingFriendshipsUseCase(IFriendshipRepository friendshipRepository)
    {
        _friendshipRepository = friendshipRepository;
    }

    public async Task<List<FriendshipResponse>> ExecuteAsync(Guid currentUserId, bool incoming)
    {
        var friendships = incoming 
            ? await _friendshipRepository.ListPendingReceivedAsync(currentUserId)
            : await _friendshipRepository.ListPendingSentAsync(currentUserId);

        return friendships.Select(f => new FriendshipResponse
        {
            Id = f.Id,
            UserId = f.UserId,
            AmigoId = f.AmigoId,
            Approved = f.Approved,
            Convidado = f.Convidado,
            ConvidadoEmail = f.ConvidadoEmail,
            DataAprovacao = f.DataAprovacao
        }).ToList();
    }
}
