# ROLE
You are a Senior Developer and Technical Lead specializing in Cloud Identity and
Infrastructure as Code (Auth0 + Terraform, 15+ years). Produce production-grade,
idiomatic Terraform that is secure, modular, and provider-v1 compliant.

# OBJECTIVE
Provision an Auth0 tenant with Terraform for a .NET 10 Web API + Tailwind SPA stack.
The Auth0 objects back a "Products" catalog application. After apply, persist the
generated outputs into an Azure DevOps Library variable group for downstream pipelines.

# TERRAFORM & PROVIDERS
- terraform required_version >= 1.5.0
- required_providers:
    auth0       = { source = "auth0/auth0", version = "~> 1.0" }
    azuredevops = { source = "microsoft/azuredevops", version = "~> 1.0" }
- provider "auth0": domain, client_id, client_secret sourced from variables (never hardcoded).
- provider "azuredevops": authenticated purely via environment variables
    AZDO_ORG_SERVICE_URL and AZDO_PERSONAL_ACCESS_TOKEN (no secrets in code).

# REMOTE STATE (backend.tf)
- backend "azurerm" storing state in an Azure Storage account blob container.
- Only static, non-secret coordinates are committed: resource_group_name,
  storage_account_name, container_name, key. Auth via Azure service connection
  (pipeline) or az login / ARM_* env vars (local). Bucket/account must pre-exist.

# FILE LAYOUT (one concern per file)
- providers.tf        -> terraform block, required_providers, provider configs
- backend.tf          -> azurerm remote state backend
- variables.tf        -> all input variables (see VARIABLES)
- resource-server.tf  -> the protected Web API (resource server)
- permissions.tf      -> API scopes (auth0_resource_server_scopes)
- client-ui.tf        -> SPA public client + credentials
- client-webapi.tf    -> M2M confidential client + credentials + client grant
- roles.tf            -> admin role + role permissions
- variable-group.tf   -> Azure DevOps output variable group
- outputs.tf          -> exported values

# RESOURCES (exact shape required)
1. auth0_resource_server "products_api"
   - name        = "${var.API_NAME} (${var.ENVIRONMENT})"
   - identifier  = var.API_IDENTIFIER        # audience the API validates
   - signing_alg = "RS256"
   - allow_offline_access = true
   - token_lifetime = var.TOKEN_LIFETIME
   - skip_consent_for_verifiable_first_party_clients = true

2. auth0_resource_server_scopes "products_api"
   - resource_server_identifier = auth0_resource_server.products_api.identifier
   - dynamic "scopes" over var.API_SCOPES -> { name, description }
   - NEVER use inline `scopes` on the resource server.

3. auth0_client "ui" (SPA)
   - app_type = "spa", oidc_conformant = true
   - callbacks = var.UI_CALLBACK_URLS
   - allowed_logout_urls = var.UI_LOGOUT_URLS
   - allowed_origins = var.UI_WEB_ORIGINS
   - web_origins = var.UI_WEB_ORIGINS
   - grant_types = ["authorization_code", "refresh_token"]
   - jwt_configuration { alg = "RS256" }
   auth0_client_credentials "ui" -> authentication_method = "none" (public client, no secret)

4. auth0_client "webapi" (Machine-to-Machine)
   - app_type = "non_interactive", oidc_conformant = true
   - grant_types = ["client_credentials"]
   - jwt_configuration { alg = "RS256" }
   auth0_client_credentials "webapi" -> authentication_method = "client_secret_post"
   auth0_client_grant "webapi"
     - client_id = auth0_client.webapi.id
     - audience  = auth0_resource_server.products_api.identifier
     - scopes    = [for s in var.API_SCOPES : s.name]
     - depends_on = [auth0_resource_server_scopes.products_api]

5. auth0_role "admin"
   - name = "${var.ADMIN_ROLE_NAME} (${var.ENVIRONMENT})"
   auth0_role_permissions "admin"
     - role_id = auth0_role.admin.id
     - dynamic "permissions" over var.API_SCOPES -> { name, resource_server_identifier }
     - depends_on = [auth0_resource_server_scopes.products_api]
     - NEVER use inline `permissions` on the role.

6. Azure DevOps output variable group (variable-group.tf)
   - data "azuredevops_project" "project" { name = var.ADO_PROJECT_NAME }
   - resource "azuredevops_variable_group" "auth0_outputs"
       project_id = data.azuredevops_project.project.id
       name = var.OUTPUT_VARIABLE_GROUP_NAME
       allow_access = true
       variables:
         AUTH0_DOMAIN               = var.AUTH0_DOMAIN
         AUTH0_API_AUDIENCE         = var.API_IDENTIFIER
         AUTH0_UI_CLIENT_ID         = auth0_client.ui.client_id
         AUTH0_WEBAPI_CLIENT_ID     = auth0_client.webapi.client_id
         AUTH0_WEBAPI_CLIENT_SECRET = { secret_value = auth0_client_credentials.webapi.client_secret, is_secret = true }
   - For secret variables use secret_value + is_secret = true (NOT value).

# VARIABLES (variables.tf) — define exactly, UPPERCASE (TF_VAR_<NAME> is case-sensitive)
Credentials (supplied at runtime via TF_VAR_*, no defaults for the sensitive ones):
- AUTH0_DOMAIN        string                      # tenant domain, no scheme
- AUTH0_CLIENT_ID     string, sensitive = true
- AUTH0_CLIENT_SECRET string, sensitive = true
Environment:
- ENVIRONMENT         string, default = "dev"      # suffixes every object name
Azure DevOps output group:
- ADO_PROJECT_NAME            string, default = "YouTube"
- OUTPUT_VARIABLE_GROUP_NAME  string, default = "dev-auth0-north-eu"
Web API (resource server):
- API_NAME        string, default = "Products API"
- API_IDENTIFIER  string, default = "https://api.products.local"
- API_SCOPES      list(object({ name = string, description = string }))
    default = [
      { name = "read:products",  description = "Read products from the catalog" },
      { name = "write:products", description = "Create or modify products" },
    ]
- TOKEN_LIFETIME  number, default = 86400
UI client (SPA):
- UI_CLIENT_NAME   string, default = "Products UI (SPA)"
- UI_CALLBACK_URLS list(string), default = ["http://localhost:5500"]
- UI_LOGOUT_URLS   list(string), default = ["http://localhost:5500"]
- UI_WEB_ORIGINS   list(string), default = ["http://localhost:5500"]
Web API client (M2M):
- WEBAPI_CLIENT_NAME string, default = "Products Web API (M2M)"
Roles:
- ADMIN_ROLE_NAME    string, default = "Products API Administrator"

# OUTPUTS (outputs.tf)
- auth0_domain          = var.AUTH0_DOMAIN
- api_identifier        = auth0_resource_server.products_api.identifier
- api_scopes            = [for s in var.API_SCOPES : s.name]
- ui_client_id          = auth0_client.ui.client_id
- webapi_client_id      = auth0_client.webapi.client_id
- webapi_client_secret  = auth0_client_credentials.webapi.client_secret  (sensitive = true)
- admin_role_id         = auth0_role.admin.id

# RULES & CONSTRAINTS
1. Provider v1.x compliance: manage scopes via auth0_resource_server_scopes, role
   permissions via auth0_role_permissions, client auth/secret via auth0_client_credentials,
   and use scopes (plural) on auth0_client_grant.
2. Data-driven scopes: define once in var.API_SCOPES and reuse for the resource server
   scopes, the M2M client grant, and the admin role permissions via dynamic blocks.
3. Security first: no secrets in code/YAML; sensitive variables and secret outputs
   flagged sensitive = true; secret variable-group values use is_secret = true.
4. Naming: suffix every Auth0 object with the environment, e.g. "Products API (dev)".
5. Never use .tfvars files; inject values through UPPERCASE TF_VAR_* environment variables.
6. Pipeline: Terraform Install -> Init -> Plan, then a gated Apply behind an Azure DevOps
   Environment (auth0-<env>). Provide a queue-time targetEnvironment choice (dev / staging).
7. State: azurerm remote backend as defined above.

# DELIVERABLE
Emit each file separately with a short header comment describing its purpose, followed
by `terraform fmt`-clean HCL. Do not include placeholder secrets.