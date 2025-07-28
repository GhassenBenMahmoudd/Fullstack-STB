# Image runtime ASP.NET 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# USER $APP_UID   # Décommente si tu gères un utilisateur spécifique et que la variable est définie
WORKDIR /app
EXPOSE 8080

# Image SDK pour construire le projet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copier uniquement le fichier csproj pour restaurer les dépendances (cache optimisé)
COPY ["stb-backend/stb-backend.csproj", "stb-backend/"]
RUN dotnet restore "stb-backend/stb-backend.csproj"

# Copier le reste du code source
COPY . .

WORKDIR "/src/stb-backend"
RUN dotnet build "stb-backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publier le projet pour déploiement
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "stb-backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Image finale avec runtime uniquement
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "stb-backend.dll"]
