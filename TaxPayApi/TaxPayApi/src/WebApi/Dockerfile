FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TaxPayApi/TaxPayApi/src/WebApi/WebApi.csproj", "TaxPayApi/src/WebApi/"]
COPY ["TaxPayApi/TaxPayApi/src/Application/Application.csproj", "TaxPayApi/src/Application/"]
COPY ["TaxPayApi/TaxPayApi/src/Domain/Domain.csproj", "TaxPayApi/src/Domain/"]
COPY ["TaxPayApi/TaxPayApi/src/Infrastructure/Infrastructure.csproj", "TaxPayApi/src/Infrastructure/"]
RUN dotnet restore "TaxPayApi/src/WebApi/WebApi.csproj"
COPY . .
WORKDIR "/src/TaxPayApi/TaxPayApi/src/WebApi"
RUN dotnet build "WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]
