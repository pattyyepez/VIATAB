FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy the solution and projects
COPY *.sln .
COPY WebApi/ WebApi/
COPY Database/ Database/
COPY Entities/ Entities/
COPY DTOs/ DTOs/

# Install EF Tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# Run restore
WORKDIR /src/WebApi
RUN dotnet restore

# Run migrations!
RUN dotnet ef database update --project ../Database --startup-project . || echo "Migration failed"

# Build & publish
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

# Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "WebApi.dll"]
