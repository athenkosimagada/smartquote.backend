# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:${PORT}
EXPOSE 5000

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and solution for restore
COPY smartquote.sln ./
COPY smartquote.api/smartquote.api.csproj ./smartquote.api/
RUN dotnet restore "./smartquote.api/smartquote.api.csproj"

# Copy the entire repo
COPY . .

# Build
WORKDIR /src/smartquote.api
RUN dotnet build "smartquote.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "smartquote.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "smartquote.api.dll"]
