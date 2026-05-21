# KuroOpti

Graduation Project – Fullstack system (React + .NET + MySQL)

# Reikalavimai

## 1. .NET 10 SDK

Reikalingas BackEnd projektams.

https://dotnet.microsoft.com/en-us/download

## 2. Node.js

Reikalingas React FrontEnd projektui.

https://nodejs.org/

## 3. Docker Desktop

Reikalingas MySQL konteinerio paleidimui.

https://www.docker.com/products/docker-desktop/

### Įsirašyti React projekto dependencies

\*leidžiama iš KuroOpti/FrontEnd/ direktorijos

```
npm install --legacy-peer-deps

```

## Projekto paleidimas eilės tvarka 1-5

#### 1. Docker konteinerio sukūrimo komanda

\*leidžiama iš KuroOpti direktorijos

```
docker compose up -d

```

#### 2. BackEnd DB migration

\*leidžiama iš KuroOpti/BackEnd/ direktorijos

```
dotnet ef database update -p KuroOpti.Data -s KuroOpti.Console
```

#### 3. Console projekto paleidimas

paima kuro kainų duomenis iš interneto
importuoja juos į MySQL duombazę

\*leidžiama iš KuroOpti/BackEnd/ direktorijos

```
dotnet run --project KuroOpti.Console
```

\*arba iš KuroOpti/BackEnd/KuroOpti.Console direktorijos

```
dotnet run
```

#### 4. API projekto paleidimas

\*atskiras bash, leidžiama iš KuroOpti/BackEnd/ direktorijos

```
dotnet run --project KuroOpti.API
```

\*atskiras bash, arba iš KuroOpti/BackEnd/KuroOpti.API direktorijos

```
dotnet run
```

#### 5. React projekto paleidimas

\* atskiras bash, leidžiama iš KuroOpti/FrontEnd/ direktorijos

```
npm run dev
```
