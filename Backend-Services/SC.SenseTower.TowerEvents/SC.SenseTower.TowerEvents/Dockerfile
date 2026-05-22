#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SC.SenseTower.TowerEvents/SC.SenseTower.TowerEvents.csproj", "SC.SenseTower.TowerEvents/"]
RUN dotnet restore "SC.SenseTower.TowerEvents/SC.SenseTower.TowerEvents.csproj"
COPY . .
WORKDIR "/src/SC.SenseTower.TowerEvents"
RUN dotnet build "SC.SenseTower.TowerEvents.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SC.SenseTower.TowerEvents.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SC.SenseTower.TowerEvents.dll"]