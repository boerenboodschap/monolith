FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS publish
WORKDIR /src
COPY . .
RUN dotnet restore /ignoreprojectextensions:.dcproj
WORKDIR /src
RUN dotnet publish bb-api.csproj -c Release -o /app

FROM base AS final
WORKDIR  /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "bb-api.dll"]
