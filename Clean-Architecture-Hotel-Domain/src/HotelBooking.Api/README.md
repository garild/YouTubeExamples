# API Layer (Presentation)

This is the entry point of the application.
It contains:
- Controllers / Minimal APIs
- Middlewares
- Dependency Injection Registration (Wiring it all up)

**Rules:**
- Depends on the Application layer.
- Should NOT contain business logic. Just receives HTTP requests and passes them to the Application layer.
