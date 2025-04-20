# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar tudo
COPY . .

# Restaurar dependências da solução completa
RUN dotnet restore TaskManager.sln

# Publicar o projeto principal
RUN dotnet publish TaskManager.API/TaskManager.API.csproj -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TaskManager.API.dll"]
