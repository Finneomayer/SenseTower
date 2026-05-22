#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SC.SenseTower.Halls/SC.SenseTower.Halls.csproj", "SC.SenseTower.Halls/"]
RUN dotnet restore "SC.SenseTower.Halls/SC.SenseTower.Halls.csproj"
COPY . .
WORKDIR "/src/SC.SenseTower.Halls"
RUN dotnet build "SC.SenseTower.Halls.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SC.SenseTower.Halls.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SC.SenseTower.Halls.dll"]