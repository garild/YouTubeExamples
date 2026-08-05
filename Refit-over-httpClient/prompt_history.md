# Prompt History

This document tracks the prompts used to generate and modify this project.

### Prompt 1: Initial Project Setup
```text
Act as a Senior React Developer, Frontend Architect, and UX Engineer.

Create a modern, production-ready React application with Auth0 authentication integration in folder frontend

Requirements Authentication:
- Use Auth0 as the authentication provider.
- Users must authenticate through the hosted Auth0 Universal Login page.
- Implement secure login and logout functionality.
- Persist the authentication session across page refreshes.
- Protect authenticated routes using Auth0 authorization mechanisms.
- Redirect unauthenticated users to the Auth0 login page.
- Create a env file which store AUTH0 Domain ClientId and CallbackUrl
```

### Prompt 2: Auth0 Authentication Configuration Update

```text
I need to update the Auth0 configuration in the React application. 

Currently, the application uses the Auth0Provider with the following configuration:
- domain: import.meta.env.VITE_AUTH0_DOMAIN || "YOUR_AUTH0_DOMAIN"
- clientId: import.meta.env.VITE_AUTH0_CLIENT_ID || "YOUR_AUTH0_CLIENT_ID"
- authorizationParams: redirect_uri: callbackUrl

Update the Auth0Provider to also include the audience parameter if it exists in the environment variables.

1. Read the audience from the environment variable VITE_AUTH0_AUDIENCE
2. If the audience is present, add it to the authorizationParams in the Auth0Provider configuration
3. Do not remove or modify the existing domain, clientId, or redirect_uri configuration
4. The audience should be added as audience: <audience-value> in the authorizationParams object
```

### Prompt 3: Build Secured BookingAPI

```text
Act as a Senior .NET and React Developer.

Build a small web api called BookingAPI that demonstrates authentication with Auth0, a secured .NET Minimal API (directory /src)

Goal:
1. Create an application, .net API  where:

- The React application calls a secured .NET Minimal API.
- The API returns the authenticated user's bookings.
- The bookings are displayed in a modern dashboard.
- Seed data with booking records
- Allow fetch and create
- All endpoint should be secured
- Configurate the Auth0 in appsetting.json
```

### Prompt 4: Refactor API to use EF Core Code-First

```text
Act as a .NET EF Core and Clean Architecture Expert.

I need to refactor the existing BookingAPI project to use Entity Framework Core with Code-First development.

Goal:
1. Replace the current in-memory "repository" with a proper EF Core implementation.
2. Use Entity Framework Core with Code-First approach
3. Add a PostgreSQL database as the data store (using Npgsql provider)
4. Refactor the application to use Clean Architecture (Domain, Application, Infrastructure layers)
5. Keep all existing functionality (authentication, endpoints) the same
6. Add seed data using EF Core DbInitializer
7. All endpoints must remain secured with Auth0

Step 1: Update Project Structure
Create a solution WaylaAI.sln with the following structure:

WaylaAI/
│
├── src/
│   ├── WaylaAI.Booking.Api/            # Minimal API project (unchanged functionality)
│   │
│   ├── WaylaAI.Booking.Application/      # Application layer with MediatR commands/queries
│   │   ├── Interfaces/
│   │   ├── UseCases/
│   │   └── DTOs/
│   │
│   ├── WaylaAI.Booking.Domain/         # Domain layer with entity and value objects
│   │   ├── Entities/
│   │   └── Interfaces/
│   │
│   └── WaylaAI.Booking.Infrastructure/ # Infrastructure layer with EF Core
│       ├── Database/
│       ├── Migrations/
│       └── Repositories/
│
├── docs/
│   ├── ARCHITECTURE.md                 # Clean Architecture documentation
│   ├── DATABASE.md                     # Database schema and migration info
│   └── API_DOCUMENTATION.md            # API endpoints with security info
│
├── WaylaAI.sln                         # Solution file
│
├── WaylaAI.Booking.sln.user            # User preferences (gitignore this)
└── .gitignore                          # Exclude bin, obj, .user files, etc.
```

### Prompt 5: Create and Integrate PaymentAPI

```text
Act as a .NET EF Core and Clean Architecture Expert.

I need to refactor the existing PaymentAPI project to use Entity Framework Core with Code-First development.(directory /src)

Goal:
1. Replace the current in-memory "repository" with a proper EF Core implementation.
2. Use Entity Framework Core with Code-First approach
3. Add a database as the data store (in memory)
4. Craete just monolit project with API and structure folder with  Clean architecture aproach
5. Keep all existing functionality (authentication, endpoints) the same
6. All endpoints must remain secured with Auth0
7. Create HttpClient in Booking API which is send the request to proces payment
```