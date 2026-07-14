# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["TaskManagementSystem.slnx", "."]
COPY ["src/TaskManagementSystem.Web/TaskManagementSystem.Web.csproj", "src/TaskManagementSystem.Web/"]
COPY ["src/TaskManagementSystem.Core/TaskManagementSystem.Core.csproj", "src/TaskManagementSystem.Core/"]
COPY ["src/TaskManagementSystem.Infrastructure/TaskManagementSystem.Infrastructure.csproj", "src/TaskManagementSystem.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "TaskManagementSystem.slnx"

# Copy entire source
COPY . .

# Publish stage
FROM build AS publish
RUN dotnet publish "src/TaskManagementSystem.Web/TaskManagementSystem.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:$PORT

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:${PORT:-8080}/health || exit 1

# Run application
ENTRYPOINT ["dotnet", "TaskManagementSystem.Web.dll"]
