FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EmailNotifications.WebApi.csproj", "EmailNotifications.WebApi/"]
COPY . ./
RUN dotnet restore "EmailNotifications.WebApi/EmailNotifications.WebApi.csproj"
WORKDIR "/src/EmailNotifications.WebApi"
RUN dotnet build "EmailNotifications.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EmailNotifications.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmailNotifications.WebApi.dll"]
