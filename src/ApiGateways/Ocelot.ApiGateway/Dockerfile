FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ApiGateways/Ocelot.ApiGateway/Ocelot.ApiGateway.csproj", "src/ApiGateways/Ocelot.ApiGateway/"]
RUN dotnet restore "./src/ApiGateways/Ocelot.ApiGateway/Ocelot.ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/ApiGateways/Ocelot.ApiGateway"
RUN dotnet build "./Ocelot.ApiGateway.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ocelot.ApiGateway.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ocelot.ApiGateway.dll"]