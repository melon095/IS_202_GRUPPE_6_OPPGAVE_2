# --------------------------------
# Bygge react prosjektet
FROM docker.io/node:24-alpine AS frontend-build
WORKDIR /src/
ARG NODE_ENV=production

COPY ["Gruppe6Oppgave2/map-ui/package.json", "Gruppe6Oppgave2/map-ui/pnpm-lock.yaml", "./"]

RUN corepack enable && corepack prepare pnpm@10.21 --activate
RUN pnpm install --frozen-lockfile

COPY ["Gruppe6Oppgave2/map-ui/", "./"]
RUN pnpm run build

# --------------------------------
# Legge til ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

# --------------------------------
# Bygge ASP.NET Core applikasjonen
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Gruppe6Oppgave2/Gruppe6Oppgave2.csproj", "Gruppe6Oppgave2/"]
RUN dotnet restore "./Gruppe6Oppgave2/Gruppe6Oppgave2.csproj"
COPY . .

COPY --from=frontend-build /wwwroot/ ./Gruppe6Oppgave2/wwwroot/
WORKDIR "/src"
RUN dotnet build "./Gruppe6Oppgave2/" -c $BUILD_CONFIGURATION -o /app/build

# --------------------------------
# Publisere applikasjonen
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Gruppe6Oppgave2" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --------------------------------
# Kjøre applikasjonen
FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080 DOTNET_RUNNING_IN_CONTAINER=true
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gruppe6Oppgave2.dll"]

# --------------------------------
