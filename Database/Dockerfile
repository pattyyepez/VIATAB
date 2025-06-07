# Use the .NET SDK image to build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ../*.sln .
COPY ../Entities/ ./Entities/
COPY ../DTOs/ ./DTOs/

# Restore dependencies, build, and publish the project
WORKDIR /src/Database
RUN dotnet restore
RUN dotnet build -c Release -o /app/build

# Publish to a folder
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Use the runtime image to copy the built app
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .