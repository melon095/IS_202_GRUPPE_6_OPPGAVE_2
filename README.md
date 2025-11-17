# Kartverket

Denne repoen inneholder koden for Gruppe 6 sin IS-202 Obligatorisk Oppgave 2

Du kan starte prosjektet ved å kjøre følgende kommandoer:

```bash
docker compose up
```

# Drift

Applikasjonen kan enten startes manuelt via Visual Studio, eller med docker ved bruk av kommando `docker compose up` i terminalen.
Den kobler seg til en MariaDB Database der tabeller blir skapt automatisk fra https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Database/DatabaseContext.cs

# Systemarkitektur

## Databaser

[DatabaseContext](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_1/blob/main/Databases/DatabaseContext.cs) klasse som kommuniserer mellom C# og databasen.

## Modeller

[HindranceObjectTable](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Database/Tables/HindranceObjectTable.cs) representerer tabellen i databasen som lagrer informasjon om hindringer.

[HindrancePointTable](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Database/Tables/HindrancePointTable.cs) representerer tabellen i databasen som lagrer geografiske punkter for hindringer.

[HindranceTypeTable](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Database/Tables/HindranceTypeTable.cs) representerer tabellen i databasen som lagrer hva slags type en hindring er.

[ReportFeedbackTable](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Database/Tables/ReportFeedbackTable.cs) representerer tabellen i databasen som lagrer tilbakemeldinger på rapporterte hindringer.

[ReportTable](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Database/Tables/ReportTable.cs) representerer tabellen i databasen som lagrer en samling av hindringer.

[RoleTable](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Database/Tables/RoleTable.cs) representerer tabellen i databasen som lagrer brukerroller.

[UserTable](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Database/Tables/UserTable.cs) representerer tabellen i databasen som lagrer brukerinformasjon.

## Kontrollere

[AdminController](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Controllers/AdminController.cs) håndterer administrasjon spesifikke funksjoner for registrerte hindringer.

[HomeController](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Controllers/HomeController.cs) håndterer startsiden.

[MapController](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Controllers/MapController.cs) håndterer kartvisning.

[ObjectTypesController](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Controllers/ObjectTypesController.cs) håndterer typer til hindringer.

[ReportController](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Controllers/ReportController.cs) håndterer tilbakemeldinger av hindringer.

[UserController](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Controllers/UserController.cs) handterer brukerrelaterte funksjoner som registrering og pålogging.

## Views

### User

[Login](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Views/User/Login.cshtml) - Side for brukerpålogging.

[Register](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Views/User/Register.cshtml) - Side for brukerregistrering.

[AccessDenied](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Views/User/AccessDenied.cshtml) - Side som vises når en bruker prøver å få tilgang til en side de ikke har tillatelse til.

### Report

[Index](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Views/Report/Index.cshtml) - Side for å se alle rapporterte hindringer.

[Details](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Views/Report/Details.cshtml) - Side for å se detaljer om en rapport.

[Object](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Views/Report/Object.cshtml) - Side for å se detaljer om en spesifikk hindring.

[_Map{Container,Scripts,Styles}Partial](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/tree/main/Gruppe6Oppgave2/Views/Report) - Delvise visninger for å integrere kart i rapportvisningene.

[ErrorView](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Views/Report/ErrorView.cshtml) - Side som vises ved feil.

### Map

[Index](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Views/Map/Index.cshtml) - Side for å vise kart for å registrere hindringer.

### Home

[Index](https://github.com/melon095/IS_202_GRUPPE_6_OPPGAVE_2/blob/main/Gruppe6Oppgave2/Views/Home/Index.cshtml) - Startsiden for applikasjonen.

# Database

Applikasjonen bruker Entity Framework Core til abstraksjon av MariaDB database samt automatisk migrere database tabeller.

Dette blir brukt for å lagre og hente data fra MariaDB database.

# Test

Enhetstester er implementert ved bruk av xUnit rammeverket. Testene dekker serviselaget.

## Lokal utvikling

For å gjøre lokal utvikling krever det at du har Node.js og pnpm installert.

1. Installer Node.js fra [nodejs.org](https://nodejs.org/). Last ned `.msi` filen.
2. Lukk terminalen og åpne en ny slik at PATH oppdateres.
3. Aktiver Corepack og PNPM:

```bash
corepack enable pnpm
```

4. Gå til prosjektet og installer avhengighetene:

```bash
cd Kartverket.Web/map-ui
pnpm install
```
