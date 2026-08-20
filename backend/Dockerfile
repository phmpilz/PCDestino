FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
WORKDIR /src

COPY backend/PCDestino.Backend.sln backend/dotnet-tools.json ./backend/
COPY backend/src/PCDestino.Domain/PCDestino.Domain.csproj ./backend/src/PCDestino.Domain/
COPY backend/src/PCDestino.Application/PCDestino.Application.csproj ./backend/src/PCDestino.Application/
COPY backend/src/PCDestino.Infrastructure/PCDestino.Infrastructure.csproj ./backend/src/PCDestino.Infrastructure/
COPY backend/src/PCDestino.Api/PCDestino.Api.csproj ./backend/src/PCDestino.Api/
RUN dotnet restore backend/src/PCDestino.Api/PCDestino.Api.csproj

COPY backend/ ./backend/
RUN dotnet publish backend/src/PCDestino.Api/PCDestino.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled-extra AS runtime
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_gcServer=1
EXPOSE 8080
COPY --from=build --chown=app:app /app/publish .
USER app
ENTRYPOINT ["dotnet", "PCDestino.Api.dll"]
