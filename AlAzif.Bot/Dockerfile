FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM base AS final
WORKDIR /app
COPY ./bin/publish .
ENTRYPOINT ["dotnet", "AlAzif.Bot.dll"]
