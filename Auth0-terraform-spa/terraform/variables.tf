# ==========================================
# INPUT VARIABLES
# ==========================================
# Names are UPPERCASE on purpose: Terraform's TF_VAR_<NAME> lookup is case-sensitive,
# so `TF_VAR_AUTH0_CLIENT_SECRET` binds to `variable "AUTH0_CLIENT_SECRET"`.

# ---- Management API credentials (delivered via TF_VAR from the DevOps Library group) ----
variable "AUTH0_DOMAIN" {
  type        = string
  description = "Auth0 tenant domain for the Management API, e.g. your-tenant.eu.auth0.com (no scheme). Supply via TF_VAR_AUTH0_DOMAIN."
}

variable "AUTH0_CLIENT_ID" {
  type        = string
  description = "Client ID of the Auth0 Management API (M2M) application. Supply via TF_VAR_AUTH0_CLIENT_ID."
  sensitive   = true
}

variable "AUTH0_CLIENT_SECRET" {
  type        = string
  description = "Client secret of the Auth0 Management API (M2M) application. Supply via TF_VAR_AUTH0_CLIENT_SECRET."
  sensitive   = true
}

# ---- Environment ----
variable "ENVIRONMENT" {
  type        = string
  description = "Target environment used to suffix resource names (e.g. dev, staging). Supply via TF_VAR_ENVIRONMENT."
  default     = "dev"
}

# ---- Azure DevOps output variable group ----
variable "ADO_PROJECT_NAME" {
  type        = string
  description = "Azure DevOps project that owns the output variable group."
  default     = "YouTube"
}

variable "OUTPUT_VARIABLE_GROUP_NAME" {
  type        = string
  description = "Name of the Azure DevOps Library variable group that receives the Auth0 outputs. Supply via TF_VAR_OUTPUT_VARIABLE_GROUP_NAME."
  default     = "dev-auth0-north-eu"
}

# ---- Web API (resource server) ----
variable "API_NAME" {
  type        = string
  description = "Display name of the protected Web API (resource server)."
  default     = "Products API"
}

variable "API_IDENTIFIER" {
  type        = string
  description = "Unique identifier (audience) of the Web API. Must match the backend Auth0 audience."
  default     = "https://api.products.local"
}

variable "API_SCOPES" {
  type = list(object({
    name        = string
    description = string
  }))
  description = "Permissions (scopes) exposed by the Web API."
  default = [
    { name = "read:products", description = "Read products from the catalog" },
    { name = "write:products", description = "Create or modify products" },
  ]
}

variable "TOKEN_LIFETIME" {
  type        = number
  description = "Access token lifetime (seconds) issued for the Web API."
  default     = 86400
}

# ---- UI client (SPA) ----
variable "UI_CLIENT_NAME" {
  type        = string
  description = "Display name of the single-page application (frontend) client."
  default     = "Products UI (SPA)"
}

variable "UI_CALLBACK_URLS" {
  type        = list(string)
  description = "Allowed callback URLs for the SPA after login."
  default     = ["http://localhost:5500"]
}

variable "UI_LOGOUT_URLS" {
  type        = list(string)
  description = "Allowed logout return URLs for the SPA."
  default     = ["http://localhost:5500"]
}

variable "UI_WEB_ORIGINS" {
  type        = list(string)
  description = "Allowed web origins (CORS) for the SPA."
  default     = ["http://localhost:5500"]
}

# ---- Web API client (machine-to-machine) ----
variable "WEBAPI_CLIENT_NAME" {
  type        = string
  description = "Display name of the backend machine-to-machine client."
  default     = "Products Web API (M2M)"
}

# ---- Roles ----
variable "ADMIN_ROLE_NAME" {
  type        = string
  description = "Display name of the administrator role granted every API permission."
  default     = "Products API Administrator"
}
