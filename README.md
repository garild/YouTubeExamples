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

### [HealthCheck-Examples](./HealthCheck-Examples)

ASP.NET Core minimal API sample that demonstrates built-in health checks with disk space, external API, and simulated HTTP responses.

**Stack:** .NET 10, ASP.NET Core health checks, `IHttpClientFactory`

**What it shows:**
- Custom `IHealthCheck` implementations for disk space and remote HTTP endpoints
- GitHub API reachability check (requires a `User-Agent` header)
- Three httpbin checks running side by side: healthy (200), unhealthy (500), and timeout (`delay/5`)
- JSON health reports at `/health`, `/health/ready`, and `/health/httpbin`
- Health Checks UI dashboard at `/health-ui`

**Run it:**

```bash
cd HealthCheck-Examples/HealthCheck.Examples
dotnet run
```

**Health endpoints:**

| Route | Description |
|-------|-------------|
| `GET` | `/health` | Runs all checks and returns a JSON report |
| `GET` | `/health/ready` | Runs checks tagged as `ready` |
| `GET` | `/health/httpbin` | Runs the three httpbin demo checks only |
| `GET` | `/health-ui` | Visual dashboard for monitoring health status |

Default URL: `http://localhost:5264`

**httpbin demo checks** (configured in `appsettings.json`):

| Check | URL | Expected result |
|-------|-----|-----------------|
| `httpbin_200` | `https://httpbin.org/status/200` | `Healthy` |
| `httpbin_500` | `https://httpbin.org/status/500` | `Unhealthy` |
| `httpbin_timeout` | `https://httpbin.org/delay/5` | `Unhealthy` (3s timeout) |
