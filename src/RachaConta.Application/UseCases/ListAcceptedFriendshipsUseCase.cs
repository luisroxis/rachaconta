using RachaConta.Application.DTOs.Response;
using RachaConta.Core.Interfaces.Repositories;

namespace RachaConta.Application.UseCases;

public class ListAcceptedFriendshipsUseCase
{
    private readonly IFriendshipRepository _friendshipRepository;

    public ListAcceptedFriendshipsUseCase(IFriendshipRepository friendshipRepository)
    {
        _friendshipRepository = friendshipRepository;
    }

    public async Task<List<FriendshipResponse>> ExecuteAsync(Guid currentUserId)
    {
        var friendships = await _friendshipRepository.ListAcceptedAsync(currentUserId);

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
