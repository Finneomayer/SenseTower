FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SC.SenseTower.Auth.csproj", "."]
RUN dotnet restore "./SC.SenseTower.Auth.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "SC.SenseTower.Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SC.SenseTower.Auth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SC.SenseTower.Auth.dll"]