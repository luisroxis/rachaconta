using RachaConta.Application.DTOs.Request;

namespace RachaConta.Application.Validators;

public class CreateGrupoValidator
{
    public void Validate(CreateGrupoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.IdCategoria))
            throw new InvalidOperationException("ID da categoria é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new InvalidOperationException("Nome do grupo é obrigatório.");
    }
}
