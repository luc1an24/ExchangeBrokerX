FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["ExchangeBrokerX.Api/ExchangeBrokerX.Api.csproj", "ExchangeBrokerX.Api/"]
COPY ["ExchangeBrokerX.Core/ExchangeBrokerX.Core.csproj", "ExchangeBrokerX.Core/"]

RUN dotnet restore "./ExchangeBrokerX.Api/ExchangeBrokerX.Api.csproj"

COPY . .

WORKDIR "/src/ExchangeBrokerX.Api"
RUN dotnet build "./ExchangeBrokerX.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ExchangeBrokerX.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY ["orderBooks.json", "exchangeBalances.json", "./"]

ENTRYPOINT ["dotnet", "ExchangeBrokerX.Api.dll"]