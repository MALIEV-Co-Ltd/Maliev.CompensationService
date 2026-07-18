# syntax=docker/dockerfile:1.7
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY --from=aspire . /Maliev.Aspire/
COPY --from=messaging . /Maliev.MessagingContracts/
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]
COPY ["Maliev.CompensationService.Api/Maliev.CompensationService.Api.csproj", "Maliev.CompensationService.Api/"]
COPY ["Maliev.CompensationService.Application/Maliev.CompensationService.Application.csproj", "Maliev.CompensationService.Application/"]
COPY ["Maliev.CompensationService.Domain/Maliev.CompensationService.Domain.csproj", "Maliev.CompensationService.Domain/"]
COPY ["Maliev.CompensationService.Infrastructure/Maliev.CompensationService.Infrastructure.csproj", "Maliev.CompensationService.Infrastructure/"]
COPY ["Maliev.CompensationService.Api/packages.lock.json", "Maliev.CompensationService.Api/"]
COPY ["Maliev.CompensationService.Application/packages.lock.json", "Maliev.CompensationService.Application/"]
COPY ["Maliev.CompensationService.Domain/packages.lock.json", "Maliev.CompensationService.Domain/"]
COPY ["Maliev.CompensationService.Infrastructure/packages.lock.json", "Maliev.CompensationService.Infrastructure/"]
RUN dotnet restore "./Maliev.CompensationService.Api/Maliev.CompensationService.Api.csproj" \
    --configfile ./nuget.config \
    --locked-mode

COPY . .
WORKDIR "/src/Maliev.CompensationService.Api"
RUN dotnet build "./Maliev.CompensationService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Maliev.CompensationService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
RUN chown -R app:app /app
USER app
COPY --chown=app:app --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/compensation/liveness || exit 1

ENTRYPOINT ["dotnet", "Maliev.CompensationService.Api.dll"]
