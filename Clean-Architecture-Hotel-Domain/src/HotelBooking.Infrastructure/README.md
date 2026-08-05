# Infrastructure Layer

This layer contains external concerns and implementations of interfaces defined in Application/Domain.
It contains:
- Entity Framework Core DbContext
- Database Migrations
- External API integrations
- File system access

**Rules:**
- Depends on the Application layer.
- This is where the "messy" real-world code lives, isolated from the core logic.
