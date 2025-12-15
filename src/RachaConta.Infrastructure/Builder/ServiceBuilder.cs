using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RachaConta.Application.UseCases;
using RachaConta.Core.Interfaces.Repositories;
using RachaConta.Core.Interfaces.Services;
using RachaConta.Infrastructure.Data;
using RachaConta.Infrastructure.Repositories;
using RachaConta.Infrastructure.Services;

namespace RachaConta.Infrastructure.Builder;

public static class ServiceBuilder
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<RachaContaDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("RachaConta.Infrastructure")));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IInviteRepository, InviteRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IGrupoRepository, GrupoRepository>();
        services.AddScoped<IParticipanteGrupoRepository, ParticipanteGrupoRepository>();

        // Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEncryptionService, EncryptionService>();

        // UseCases
        services.AddScoped<RegisterUserUseCase>();
        services.AddScoped<RequestPasswordRecoveryUseCase>();
        services.AddScoped<ResetPasswordUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<SendInviteUseCase>();
        services.AddScoped<ResendInviteUseCase>();
        services.AddScoped<DeleteInviteUseCase>();
        services.AddScoped<ListInvitesUseCase>();
        services.AddScoped<CreateCategoryUseCase>();
        services.AddScoped<ListCategoriesUseCase>();
        services.AddScoped<ListUsersUseCase>();
        services.AddScoped<RequestFriendshipUseCase>();
        services.AddScoped<ListPendingFriendshipsUseCase>();
        services.AddScoped<ApproveFriendshipUseCase>();
        services.AddScoped<ListAcceptedFriendshipsUseCase>();
        services.AddScoped<CreateGrupoUseCase>();
        services.AddScoped<ListGruposUseCase>();
        services.AddScoped<GetGrupoDetailsUseCase>();
        services.AddScoped<AddParticipantesGrupoUseCase>();
        services.AddScoped<PromoteParticipanteUseCase>();
        services.AddScoped<DeleteParticipanteGrupoUseCase>();

        return services;
    }
}
