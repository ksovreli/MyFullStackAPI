# 1. Base image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 2. SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BackpackStoreFS.csproj", "./"]
RUN dotnet restore "./BackpackStoreFS.csproj"
COPY . .
RUN dotnet build "BackpackStoreFS.csproj" -c Release -o /app/build

# 3. Publish the app
FROM build AS publish
RUN dotnet publish "BackpackStoreFS.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Final stage: copy published files and run
FROM base AS final
WORKDIR /app
COPY --select=publish /app/publish .
ENTRYPOINT ["dotnet", "BackpackStoreFS.dll"]