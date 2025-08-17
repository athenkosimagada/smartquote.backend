FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["smartquote.api.csproj", "./"]
RUN dotnet restore "./smartquote.api.csproj"
COPY . .
RUN dotnet build "./smartquote.api.csproj" -c Release -o /app/build