# ==========================================
# UI CLIENT (SINGLE PAGE APPLICATION)
# ==========================================
# Public browser client for the Tailwind SPA frontend. Uses Authorization Code + PKCE
# (no client secret). Authentication method is managed via auth0_client_credentials.
resource "auth0_client" "ui" {
  name            = "${var.UI_CLIENT_NAME} (${var.ENVIRONMENT})"
  description     = "Managed by Terraform"
  app_type        = "spa"
  oidc_conformant = true

  callbacks           = var.UI_CALLBACK_URLS
  allowed_logout_urls = var.UI_LOGOUT_URLS
  allowed_origins     = var.UI_WEB_ORIGINS
  web_origins         = var.UI_WEB_ORIGINS

  grant_types = ["authorization_code", "refresh_token"]

  jwt_configuration {
    alg = "RS256"
  }
}

# SPAs are public clients: no secret, token endpoint auth method "none".
resource "auth0_client_credentials" "ui" {
  client_id             = auth0_client.ui.id
  authentication_method = "none"
}
