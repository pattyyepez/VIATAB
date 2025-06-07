FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory for the build process
WORKDIR /src

# Copy the solution file (if it's one level above the Dockerfile, adjust path)
COPY ../*.sln .

# Copy the WebApi, Database, Entities, and DTOs projects into the container
COPY ../WebApi ./WebApi/
COPY ../Database ./Database/
COPY ../Entities ./Entities/
COPY ../DTOs ./DTOs/

# Set the working directory for WebApi project
WORKDIR /src/WebApi

# Restore the dependencies
RUN dotnet restore

# Install EF tools (global, so we can run migrations)
#RUN dotnet tool install --global dotnet-ef
#ENV PATH="$PATH:/root/.dotnet/tools"
#
#RUN dotnet ef database update --project /src/Database/ --startup-project /src/WebApi/
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "WebApi.dll"]