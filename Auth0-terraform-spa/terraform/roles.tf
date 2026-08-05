# ==========================================
# ROLES & PERMISSION ASSIGNMENT
# ==========================================
# Administrator role that receives every scope exposed by the Web API. Permissions
# are assigned in bulk (1:many) and stay in sync with var.API_SCOPES.
resource "auth0_role" "admin" {
  name        = "${var.ADMIN_ROLE_NAME} (${var.ENVIRONMENT})"
  description = "Full access to the Products API"
}

resource "auth0_role_permissions" "admin" {
  role_id = auth0_role.admin.id

  dynamic "permissions" {
    for_each = var.API_SCOPES
    content {
      name                       = permissions.value.name
      resource_server_identifier = auth0_resource_server.products_api.identifier
    }
  }

  depends_on = [auth0_resource_server_scopes.products_api]
}
