FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
ARG APP_UID=1000
RUN groupadd -g ${APP_UID} appgroup && \
  useradd -u ${APP_UID} -g appgroup -m -s /sbin/nologin appuser

WORKDIR /app

EXPOSE 8080
EXPOSE 8081


RUN mkdir -p /app/data /app/Uploads \
  && chown -R appuser:appgroup /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Api/Api.csproj", "Api/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Persistence/Persistence.csproj", "Persistence/"]
COPY ["CurrentUserService/CurrentUserService.csproj", "CurrentUserService/"]
COPY ["FileService/FileService.csproj", "FileService/"]
COPY ["RedisService/RedisService.csproj", "RedisService/"]
COPY ["TokenService/TokenService.csproj", "TokenService/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "Api/Api.csproj"
COPY . .
WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" \
  -c $BUILD_CONFIGURATION \
  -o /app/build \
  --no-restore 

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Api.csproj" \
  -c ${BUILD_CONFIGURATION} \
  -o /app/publish \
  /p:UseAppHost=false 

FROM base AS final
WORKDIR /app
COPY --from=publish --chown=appuser:appgroup /app/publish .

USER appuser

ENTRYPOINT ["dotnet", "Api.dll"]
