FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /WarehouseManagementApi

COPY ["Presentation/Presentation.csproj", "Presentation/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Domain/Domain.csproj", "Domain/"]

COPY .docker-nuget-cache /root/.nuget/packages

RUN dotnet restore Presentation/Presentation.csproj

COPY . .

WORKDIR /WarehouseManagementApi/Presentation
RUN dotnet publish -c Release -o /app/publish --no-restore



FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Presentation.dll"]
