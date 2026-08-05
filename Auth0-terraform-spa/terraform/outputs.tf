# ==========================================
# OUTPUTS
# ==========================================
output "auth0_domain" {
  value       = var.AUTH0_DOMAIN
  description = "Auth0 tenant domain."
}

output "api_identifier" {
  value       = auth0_resource_server.products_api.identifier
  description = "Audience (identifier) of the Web API resource server."
}

output "api_scopes" {
  value       = [for scope in var.API_SCOPES : scope.name]
  description = "Scopes exposed by the Web API."
}

output "ui_client_id" {
  value       = auth0_client.ui.client_id
  description = "Client ID of the SPA (frontend) application."
}

output "webapi_client_id" {
  value       = auth0_client.webapi.client_id
  description = "Client ID of the backend machine-to-machine application."
}

output "webapi_client_secret" {
  value       = auth0_client_credentials.webapi.client_secret
  description = "Client secret of the backend machine-to-machine application."
  sensitive   = true
}

output "admin_role_id" {
  value       = auth0_role.admin.id
  description = "ID of the administrator role."
}
