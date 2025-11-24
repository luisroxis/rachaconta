FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/RachaConta.Api/RachaConta.Api.csproj", "src/RachaConta.Api/"]
COPY ["src/RachaConta.Application/RachaConta.Application.csproj", "src/RachaConta.Application/"]
COPY ["src/RachaConta.Core/RachaConta.Core.csproj", "src/RachaConta.Core/"]
COPY ["src/RachaConta.Infrastructure/RachaConta.Infrastructure.csproj", "src/RachaConta.Infrastructure/"]
RUN dotnet restore "src/RachaConta.Api/RachaConta.Api.csproj"

COPY . .
WORKDIR "/src/src/RachaConta.Api"
RUN dotnet build "RachaConta.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RachaConta.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RachaConta.Api.dll"]
