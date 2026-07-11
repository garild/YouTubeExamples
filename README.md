# YouTubeExamples

Companion code samples for YouTube tutorials on .NET and Entity Framework Core.

## Projects

### [EF-Include-AsNotTracking-IDbContext](./EF-Include-AsNotTracking-IDbContext)

ASP.NET Core minimal API sample that demonstrates Entity Framework Core query patterns with `Include`, `AsNoTracking`, and `IDbContext`.

**Stack:** .NET 10, EF Core 10, in-memory database, lazy-loading proxies

**What it shows:**
- Layered structure (`Api`, `Application`, `Domain`, `Infrastructure`)
- Product catalog domain with related entities (categories, suppliers, images, reviews, tags)
- Read-only queries with `AsNoTracking()` vs tracked lookups with `FindAsync`
- `Result<T>` pattern mapped to HTTP responses
- Seeded in-memory data for quick local testing

**Run it:**

```bash
cd EF-Include-AsNotTracking-IDbContext/EF.Examples
dotnet run
```

**API endpoints:**

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/products` | List all products (no-tracking projection) |
| `GET` | `/api/products/{id}` | Get a single product by ID |

Default URL: `http://localhost:5256`

### [Polymorphic-Stripe-Paypal](./Polymorphic-Stripe-Paypal)

ASP.NET Core minimal API sample that demonstrates polymorphic JSON request binding for payment flows.

**Stack:** .NET 10, ASP.NET Core minimal APIs, System.Text.Json polymorphism

**What it shows:**
- `JsonPolymorphic` and `JsonDerivedType` attributes for discriminator-based request models
- A shared `PaymentRequest` base type with Stripe and PayPal-specific derived records
- Minimal API endpoint logic that pattern matches the concrete request type
- HTTP examples for posting Stripe and PayPal payment payloads

**Run it:**

```bash
cd Polymorphic-Stripe-Paypal/Payment.Examples
dotnet run
```

**API endpoints:**

| Method | Route | Description |
|--------|-------|-------------|
| `POST` | `/api/payments` | Process a polymorphic Stripe or PayPal payment request |

Default URL: `http://localhost:5260`
