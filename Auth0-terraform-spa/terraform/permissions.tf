# ==========================================
# API PERMISSIONS (SCOPES)
# ==========================================
# Manages the full set of scopes for the Web API (1:many). Driven by var.API_SCOPES
# so permissions stay data-driven and are reused when granting roles/clients.
resource "auth0_resource_server_scopes" "products_api" {
  resource_server_identifier = auth0_resource_server.products_api.identifier

  dynamic "scopes" {
    for_each = var.API_SCOPES
    content {
      name        = scopes.value.name
      description = scopes.value.description
    }
  }
}
