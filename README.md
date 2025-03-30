# livefront-challenge

## Running locally (In Memory Database)

`dotnet run --project src/CartonCaps.csproj`

## Running test (In Memory Database)

`dotnet test`

# Run with development settings (hot reload)

`docker compose -f docker-compose.yml -f docker-compose.local.yml up --build`

# Run with production settings only

`docker compose up --build`
