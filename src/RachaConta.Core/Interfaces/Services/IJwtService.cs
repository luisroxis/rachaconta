namespace RachaConta.Core.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(Entities.User user);
}
