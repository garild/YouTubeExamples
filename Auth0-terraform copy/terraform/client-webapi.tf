# ==========================================
# WEB API CLIENT (MACHINE-TO-MACHINE)
# ==========================================
# Confidential backend client used for service-to-service (client_credentials) calls
# to the Web API. Its secret is managed/read via auth0_client_credentials.
resource "auth0_client" "webapi" {
  name            = "${var.WEBAPI_CLIENT_NAME} (${var.ENVIRONMENT})"
  description     = "Managed by Terraform"
  app_type        = "non_interactive"
  oidc_conformant = true

  grant_types = ["client_credentials"]

  jwt_configuration {
    alg = "RS256"
  }
}

resource "auth0_client_credentials" "webapi" {
  client_id             = auth0_client.webapi.id
  authentication_method = "client_secret_post"
}

# Authorize the M2M client to request the Web API scopes.
resource "auth0_client_grant" "webapi" {
  client_id = auth0_client.webapi.id
  audience  = auth0_resource_server.products_api.identifier
  scopes    = [for scope in var.API_SCOPES : scope.name]

  depends_on = [auth0_resource_server_scopes.products_api]
}
