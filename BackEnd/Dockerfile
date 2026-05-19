# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore KuroOpti.Console/KuroOpti.Console.csproj

RUN dotnet publish KuroOpti.Console/KuroOpti.Console.csproj \
    -c Release \
    -o /app/publish

# runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "KuroOpti.Console.dll"]