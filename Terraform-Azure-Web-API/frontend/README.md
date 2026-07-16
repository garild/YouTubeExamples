# Frontend Local Development

The frontend is a static HTML and JavaScript application. It uses Tailwind CSS and the Auth0 SPA SDK from CDNs, so no build step is required.

## Prerequisites

- Node.js/npm or Python 3 to serve the static files
- The `Products.Api` backend running locally
- An Auth0 Single Page Application and API
- Internet access for the Tailwind CSS, Auth0 SDK, and Google Fonts CDNs

Do not open `index.html` directly with a `file://` URL. Auth0 redirects and browser CORS behavior require an HTTP origin.

## 1. Configure Auth0

Update `auth0Config` at the top of `app.js`:

```javascript
const auth0Config = {
  domain: "dev-youtube-gt.eu.auth0.com", // bare host only — no https://
  clientId: "YOUR_AUTH0_SPA_CLIENT_ID",  // AUTH0_UI_CLIENT_ID from Azure DevOps Library
  audience: "https://api.products.local" // must match API Auth0:Audience
};
```

Use the SPA client id from the Auth0 Terraform output variable group (`AUTH0_UI_CLIENT_ID`), not the Management API or Web API M2M client.

In the Auth0 SPA application settings, add the frontend URL to:

- Allowed Callback URLs: `http://localhost:5500`
- Allowed Logout URLs: `http://localhost:5500`
- Allowed Web Origins: `http://localhost:5500`

The Auth0 API audience must match `auth0Config.audience` and the backend `Auth0__Audience` value.

## 2. Start the backend

From the `Terraform-Azure-Web-API` directory, configure Auth0 and run the API:

```powershell
$env:Auth0__Domain = "https://YOUR_AUTH0_DOMAIN/"
$env:Auth0__Audience = "https://api.products.local"
dotnet run --project Products.Api
```

The current API launch profile uses:

- HTTP: `http://localhost:62254`
- HTTPS: `https://localhost:62253`

Update the local URL in `app.js` to match the API:

```javascript
const apiBaseUrl = window.location.hostname === "localhost" ||
  window.location.hostname === "127.0.0.1"
  ? "http://localhost:62254"
  : "https://app-yt-examples-api-dev.azurewebsites.net";
```

The frontend origin `http://localhost:5500` is already allowed by the backend CORS policy.

## 3. Start the frontend

From the `frontend` directory, use either option.

### Node.js

```powershell
npx serve . -l 5500
```

### Python

```powershell
python -m http.server 5500
```

Open:

```text
http://localhost:5500
```

## 4. Verify

1. Confirm the product catalog loads from `GET /api/products`.
2. Select **Sign In** and complete the Auth0 redirect.
3. Confirm the user profile and **Add Product** button appear.
4. Create a product and verify `POST /api/products` succeeds with the bearer token.

## Troubleshooting

- **Catalog cannot load:** Confirm the API is running and `apiBaseUrl` uses its actual HTTP port.
- **CORS error:** Serve the frontend from an origin allowed in `Products.Api/Program.cs`.
- **Auth0 initialization fails:** Replace `PLACEHOLDER_CLIENT_ID` and verify the domain, audience, and Auth0 application URLs.
- **Login redirect loops or fails:** Ensure the exact frontend origin is configured in all three Auth0 URL settings.
- **HTTPS certificate error:** Use the local HTTP API URL, or trust the ASP.NET Core development certificate with `dotnet dev-certs https --trust`.
