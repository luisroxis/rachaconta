using RachaConta.Application.DTOs.Request;

namespace RachaConta.Application.Validators;

public class AddParticipantesGrupoValidator
{
    public void Validate(AddParticipantesGrupoRequest request)
    {
        if (request.GrupoId == Guid.Empty)
            throw new InvalidOperationException("ID do grupo é obrigatório.");

        if (request.IdParticipantes == null || request.IdParticipantes.Count == 0)
            throw new InvalidOperationException("Pelo menos um participante deve ser adicionado.");
    }
}
