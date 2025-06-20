﻿# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and required projects
COPY ../*.sln .
COPY ../Blazor ./Blazor/
COPY ../DTOs ./DTOs/
COPY ../WebApi ./WebApi/
COPY ../Database ./Database/
COPY ../Entities ./Entities/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Publish the Blazor project
WORKDIR /src/Blazor
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080

ENTRYPOINT ["dotnet", "Blazor.dll"]
