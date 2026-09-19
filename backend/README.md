# CalendarScheduler.Api

ASP.NET Core Web API backend for the habit tracker. EF Core targets Azure SQL Database
(`Microsoft.EntityFrameworkCore.SqlServer`), auth is ASP.NET Core Identity + JWT bearer tokens.

## First-time setup

1. Create an Azure SQL Database (a free/serverless tier is fine for dev) and get its ADO.NET
   connection string.
2. Set local secrets (never committed — stored outside the repo by the .NET secret manager):

   ```
   cd CalendarScheduler.Api
   dotnet user-secrets set "ConnectionStrings:Default" "<your Azure SQL connection string>"
   dotnet user-secrets set "Jwt:SigningKey" "<a long random string, 32+ chars>"
   ```

3. Run the API:

   ```
   dotnet run
   ```

   Default dev URL: `http://localhost:5207`. In the Development environment, startup
   automatically applies pending EF Core migrations and seeds a known admin account
   (`admin@local.test` / `Admin123!`) so you can log in without registering first.
   Override the seeded credentials with:

   ```
   dotnet user-secrets set "SeedAdmin:Email" "you@example.com"
   dotnet user-secrets set "SeedAdmin:Password" "<your password>"
   ```

   Outside Development (e.g. a real deployment), migrations aren't applied automatically —
   run them explicitly instead:

   ```
   dotnet tool install --global dotnet-ef   # once, if not already installed
   dotnet ef database update
   ```

## Adding a new migration after model changes

```
dotnet ef migrations add <Name> -o Data/Migrations
dotnet ef database update
```

## Testing endpoints

Use the Bruno collection at `../bruno/CalendarScheduler` (Local environment). Run
`Auth/Register` or `Auth/Login` first — it stores the returned JWT into the `authToken`
environment variable that the `Habits` requests use.
