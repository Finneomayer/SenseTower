FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ImageAPI/ImageAPI.csproj", "ImageAPI/"]
COPY ["SC.Internship.Common/SC.Internship.Common.csproj", "SC.Internship.Common/"]
RUN dotnet restore "ImageAPI/ImageAPI.csproj"
COPY . .
WORKDIR "/src/ImageAPI"
RUN dotnet build "ImageAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ImageAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImageAPI.dll"]
