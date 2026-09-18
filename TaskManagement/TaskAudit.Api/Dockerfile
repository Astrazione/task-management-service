FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["TaskAudit.Api/TaskAudit.Api.csproj", "TaskAudit.Api/"]
COPY ["TaskManagement.Contracts/TaskManagement.Contracts.csproj", "TaskManagement.Contracts/"]

RUN dotnet restore "./TaskAudit.Api/TaskAudit.Api.csproj"
COPY . .
WORKDIR "/src/TaskAudit.Api"
RUN dotnet build "./TaskAudit.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# публикация
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TaskAudit.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# запуск
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskAudit.Api.dll"]