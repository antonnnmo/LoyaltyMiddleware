FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["LoyaltyMiddleware/RedmondLoyaltyMiddleware.csproj", "LoyaltyMiddleware/"]
RUN dotnet restore "LoyaltyMiddleware/RedmondLoyaltyMiddleware.csproj"
COPY . .
WORKDIR "/src/LoyaltyMiddleware"
RUN dotnet build "RedmondLoyaltyMiddleware.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "RedmondLoyaltyMiddleware.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RedmondLoyaltyMiddleware.dll"]