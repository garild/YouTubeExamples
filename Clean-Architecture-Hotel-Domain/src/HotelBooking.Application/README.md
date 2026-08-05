# Application Layer

This layer contains the application-specific business rules.
It contains:
- Commands and Queries (CQRS)
- DTOs
- Handlers
- Interfaces for infrastructure services (e.g. EmailService)

**Rules:**
- Depends ONLY on the Domain layer.
- No direct database access or web framework dependencies.
