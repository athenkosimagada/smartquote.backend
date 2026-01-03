# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["smartquote.api.csproj", "."]
RUN dotnet restore "./smartquote.api.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "./smartquote.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./smartquote.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000 5001
ENTRYPOINT ["dotnet", "smartquote.api.dll"]