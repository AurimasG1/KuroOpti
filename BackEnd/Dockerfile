FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy everything
COPY . .

# restore ONLY console entry project
RUN dotnet restore KuroOpti.Console/KuroOpti.Console.csproj

# publish console
RUN dotnet publish KuroOpti.Console/KuroOpti.Console.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "KuroOpti.Console.dll"]