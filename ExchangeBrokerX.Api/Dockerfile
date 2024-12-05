FROM mcr.microsoft.com/dotnet/sdk:8.0 AS core
WORKDIR /src
COPY ./ExchangeBrokerX.Core ./ExchangeBrokerX.Core
RUN dotnet build ./ExchangeBrokerX.Core/ExchangeBrokerX.Core.csproj -c Release

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ./ExchangeBrokerX.Api ./ExchangeBrokerX.Api
COPY --from=core /src/ExchangeBrokerX.Core ./ExchangeBrokerX.Core
RUN dotnet restore ./ExchangeBrokerX.Api/ExchangeBrokerX.Api.csproj
RUN dotnet publish ./ExchangeBrokerX.Api/ExchangeBrokerX.Api.csproj -c Release -o /out

COPY ./orderBooks.json ./exchangesBalance.json /app/

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ExchangeBrokerX.Api.dll"]