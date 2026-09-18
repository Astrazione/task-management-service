FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app


# сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["TaskEvents.Consumer/TaskEvents.Consumer.csproj", "TaskEvents.Consumer/"]
COPY ["TaskManagement.Contracts/TaskManagement.Contracts.csproj", "TaskManagement.Contracts/"]

RUN dotnet restore "./TaskEvents.Consumer/TaskEvents.Consumer.csproj"
COPY . .
WORKDIR "/src/TaskEvents.Consumer"
RUN dotnet build "./TaskEvents.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# публикация
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TaskEvents.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# запуск
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskEvents.Consumer.dll"]