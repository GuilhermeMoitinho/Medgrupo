FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Medgrupo.sln ./
COPY src/Medgrupo.Business/Medgrupo.Business.csproj src/Medgrupo.Business/
COPY src/Medgrupo.Data/Medgrupo.Data.csproj src/Medgrupo.Data/
COPY src/Medgrupo.WebApi/Medgrupo.WebApi.csproj src/Medgrupo.WebApi/
COPY tests/Medgrupo.UnitTests/Medgrupo.UnitTests.csproj tests/Medgrupo.UnitTests/

RUN dotnet restore src/Medgrupo.WebApi/Medgrupo.WebApi.csproj

COPY . .
RUN dotnet publish src/Medgrupo.WebApi/Medgrupo.WebApi.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Medgrupo.WebApi.dll"]
