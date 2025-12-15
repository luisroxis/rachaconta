using RachaConta.Application.DTOs.Request;

namespace RachaConta.Application.Validators;

public class PromoteParticipanteValidator
{
    public void Validate(PromoteParticipanteRequest request)
    {
        if (request.GrupoId == Guid.Empty)
            throw new InvalidOperationException("ID do grupo é obrigatório.");

        if (request.ParticipanteId == Guid.Empty)
            throw new InvalidOperationException("ID do participante é obrigatório.");
    }
}
