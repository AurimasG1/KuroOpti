# KuroOpti

Graduation Project

## Setup prieš paleidžiant projektą

- #### Įsirašyti .NET 10 SDK

  https://dotnet.microsoft.com/en-us/download

- #### Įsirašyti React projekto dependencies

  _\*leidžiama iš ./frontend/ direktorijos_

  ```
  --legacy-peer-deps
  ```

## Projekto paleidimas

#### 1. Docker konteinerio sukūrimo komanda

```
docker run --name KuroOpti -e MYSQL_ROOT_PASSWORD=root -d -p 3306:3306 mysql:lts
```

#### 2. API projekto paleidimas

\_\*leidžiama iš KuroOpti/BackEnd/KuroOpti.API/ direktorijos

```
dotnet run
```

#### 3. React projekto paleidimas

\_\*leidžiama iš KuroOpti/FrontEnd/ direktorijos

```
npm run dev
```

### DB migracijos komandos

#### Migracijų atnaujinimas rankiniu būdu

```
dotnet ef database update
```

#### Migracijos pridėjimas

_\*leidžiama iš projekto root direktorijos_  
_\*Čia pavyzdys. Kuriant naują migraciją reikia pakeisti pavadinimą_

```
dotnet ef migrations add UpdateUserTable -p ./backend/MySavings.Data/ -s ./backend/MySavings.API/
```

### Swagger nuoroda

http://localhost:5141/swagger/index.html
