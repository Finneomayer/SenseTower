#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SC.SenseTower.Utilites/SC.SenseTower.Utilites.csproj", "SC.SenseTower.Utilites/"]
RUN dotnet restore "SC.SenseTower.Utilites/SC.SenseTower.Utilites.csproj"
COPY . .
WORKDIR "/src/SC.SenseTower.Utilites"
RUN dotnet build "SC.SenseTower.Utilites.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SC.SenseTower.Utilites.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SC.SenseTower.Utilites.dll"]