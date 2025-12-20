# Stage 1: Build Angular client
FROM node:22-alpine AS client-build
WORKDIR /src

COPY src/client/dev-pairing/ .
RUN npm ci
RUN npm run build

# Stage 2: Build .NET server
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS server-build
WORKDIR /src

# Copy server source
COPY src/server/ ./server/

# Copy Angular build output to wwwroot
COPY --from=client-build /src/dist/dev-pairing/browser/ ./server/DevPairing.Api/wwwroot/

# Publish .NET server
WORKDIR /src/server/DevPairing.Api
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=server-build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "DevPairing.Api.dll"]
