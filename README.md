# KuroOpti

Graduation Project – Fullstack system (React + .NET + MySQL)

# Reikalavimai

## 1. .NET 8 SDK

Reikalingas BackEnd projektams.

https://dotnet.microsoft.com/en-us/download

## 2. EF Core CLI Tool

Reikalingas DB migracijoms.

```bash
dotnet tool install --global dotnet-ef --version 8.*
```

Patikrinimas:

```
dotnet ef
```

---

## 3. Node.js

Reikalingas React FrontEnd projektui.

https://nodejs.org/

## 4. Docker Desktop

Reikalingas MySQL konteinerio paleidimui.

https://www.docker.com/products/docker-desktop/

### Įsirašyti React projekto dependencies

\*leidžiama iš KuroOpti/FrontEnd/ direktorijos

```
npm install --legacy-peer-deps

```

# Projekto paleidimas eilės tvarka 1-5

### 1. Docker konteinerio sukūrimo komanda

Reikia tik pirmą kartą.

\*leidžiama iš KuroOpti/ direktorijos

```
docker compose up -d

```

### 2. BackEnd DB migration

Sukuria lenteles lokalioje duomenų bazėje.

\*leidžiama iš KuroOpti/ direktorijos

```
dotnet ef database update -p BackEnd/KuroOpti.Data -s BackEnd/KuroOpti.Console
```

### 3. Console projekto paleidimas

paima kuro kainų duomenis iš interneto ir importuoja juos į MySQL duombazę.
Pirmą kartą labai ilgai dėlios koordinates.
Jeigu liks tas pats Docker Image, tai antrą kartą labai greitai atsinaujins tik kuro kainos ir atnaujinimo laikas.

\*leidžiama iš KuroOpti/ direktorijos

```
dotnet run --project BackEnd/KuroOpti.Console
```

### 4. API projekto paleidimas

BackEnd serverio paleidimas.

\*atskiras bash, leidžiama iš KuroOpti/ direktorijos

```
dotnet run --project BackEnd/KuroOpti.API
```

### 5. React projekto paleidimas

FrontEnd serverio paleidimas.

\* atskiras bash, leidžiama iš KuroOpti/FrontEnd/ direktorijos

```
npm run dev
```
