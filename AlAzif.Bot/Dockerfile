FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AlAzif.Bot.csproj", "AlAzif.Bot/"]
RUN dotnet restore "AlAzif.Bot/AlAzif.Bot.csproj"
COPY . AlAzif.Bot/
WORKDIR "/src/AlAzif.Bot"
RUN dotnet build "AlAzif.Bot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AlAzif.Bot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AlAzif.Bot.dll"]
