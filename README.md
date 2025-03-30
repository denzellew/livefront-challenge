# livefront-challenge

## Documentation

### [API Specifications and Challenge Answers](./docs/APISpecs.md)

### [Full System Design](./docs/SystemDesign.md)

## How To Run the Application

### Generate a JWT

- **Random User Id:** `sh generate-fake-jwt.sh`
- **Existing User Id:** `sh generate-fake-jwt.sh {Exising-User-UUID}`

### Running locally (In Memory Database)

`dotnet run --project src/CartonCaps.csproj`

### Running test (In Memory Database)

`dotnet test`

### Run In Docker - Dev Settings

`docker compose -f docker-compose.yml -f docker-compose.local.yml up --build`

### Run In Docker - Production Settings

`docker compose up --build`
