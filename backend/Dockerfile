#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /backend
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./backend.csproj", "."]
RUN dotnet restore "./backend.csproj"
COPY . .
RUN dotnet build "./backend.csproj" -c $BUILD_CONFIGURATION -o /backend/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./backend.csproj" -c $BUILD_CONFIGURATION -o /backend/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /backend
COPY --from=publish /backend/publish .
ENTRYPOINT ["dotnet", "backend.dll"]