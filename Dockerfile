# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln . 
COPY CyberCity.Controller/*.csproj ./CyberCity.Controller/
COPY CyberCity.Application/*.csproj ./CyberCity.Application/
COPY CyberCity.Doman/*.csproj ./CyberCity.Doman/
COPY CyberCity.Infrastructure/*.csproj ./CyberCity.Infrastructure/
COPY CyberCity.DTOs/*.csproj ./CyberCity.DTOs/
COPY CyberCity_AutoMapper/*.csproj ./CyberCity_AutoMapper/

RUN dotnet restore

COPY . .
WORKDIR /src/CyberCity.Controller
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CyberCity.Controller.dll"]