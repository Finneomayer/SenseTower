#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 3000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["IdentityServer0/IdentityServer0.csproj", "IdentityServer0/"]
RUN dotnet restore "IdentityServer0/IdentityServer0.csproj"
COPY . .
WORKDIR "/src/IdentityServer0"
RUN dotnet build "IdentityServer0.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityServer0.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .	
ENV ASPNETCORE_URLS=http://*:3000 
ENTRYPOINT ["dotnet", "IdentityServer0.dll"]