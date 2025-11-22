# PROGI_Challenge

Full-stack solution composed of:

- A .NET 8 Web API for fee calculation.
- A Vue 3 frontend that calls the API and displays the breakdown.

The backend implements the business rules and test cases described in the coding challenge specification.

## Backend -> path: /backend
```bash
dotnet restore
dotnet build
dotnet run
```

The API listens on:
- http://localhost:5000

Swagger UI opens automatically at:
- http://localhost:5000/swagger

## Frontend -> path /frontend
## Was developed with node v20 
```bash
npm install
npm run dev
```

The app runs on:
- http://localhost:5173
*** During development, requests to `/api` are proxied to the backend. ***

## Tests

### Backend tests -> path: /backend.test
```bash
dotnet test
```
### Frontend tests -> path: /frontend
```bash
npm test
```
