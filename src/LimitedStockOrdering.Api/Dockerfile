FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY ["src/LimitedStockOrdering.Api/LimitedStockOrdering.Api.csproj", "src/LimitedStockOrdering.Api/"]
RUN dotnet restore "src/LimitedStockOrdering.Api/LimitedStockOrdering.Api.csproj"

COPY . .
RUN dotnet publish "src/LimitedStockOrdering.Api/LimitedStockOrdering.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LimitedStockOrdering.Api.dll"]
