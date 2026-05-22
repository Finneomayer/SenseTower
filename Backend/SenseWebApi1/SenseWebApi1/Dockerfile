#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SenseWebApi1/SenseWebApi1.csproj", "SenseWebApi1/"]
COPY ["SC.Internship.Common/SC.Internship.Common.csproj", "SC.Internship.Common/"]
RUN dotnet restore "SenseWebApi1/SenseWebApi1.csproj"
COPY . .
WORKDIR "/src/SenseWebApi1"
RUN dotnet build "SenseWebApi1.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "SenseWebApi1.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SenseWebApi1.dll"]