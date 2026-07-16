# Auth0 Provisioning with Terraform & Azure DevOps

This project provisions an Auth0 tenant with Terraform using the official [`auth0/auth0`](https://registry.terraform.io/providers/auth0/auth0/latest) provider (v1.x). Every concern is separated into its own file, and the Management API credentials are supplied at run time through `TF_VAR_*` environment variables sourced from an Azure DevOps Library variable group.

## Folder structure (example)

```text
Auth0-Terraform/
|-- terraform/
|   |-- backend.tf
|   |-- main.tf
|   |-- providers.tf
|   |-- variables.tf
|   |-- outputs.tf
|   |-- permissions.tf
|   |-- resource-server.tf
|   |-- client-ui.tf
|   |-- client-webapi.tf
|   |-- roles.tf
|   |-- variable-group.tf
|   |-- environments/
|   |   |-- dev/
|   |   |   |-- backend.tf
|   |   |   |-- main.tf
|   |   |   |-- outputs.tf
|   |   |   |-- providers.tf
|   |   |   `-- variables.tf
|   |   `-- staging/
|   |       |-- backend.tf
|   |       |-- main.tf
|   |       |-- outputs.tf
|   |       |-- providers.tf
|   |       `-- variables.tf
|   `-- modules/
|       `-- auth0/
|           |-- client-ui.tf
|           |-- client-webapi.tf
|           |-- outputs.tf
|           |-- permissions.tf
|           |-- resource-server.tf
|           |-- roles.tf
|           |-- variable-group.tf
|           |-- variables.tf
|           `-- versions.tf
|-- pipeline-auth0.yml
|-- master_prompt.md
`-- README.md
```

- `terraform/*.tf` — main/root configuration, matching the current split-by-concern layout
- `terraform/environments/<env>` — root modules (one state file per environment)
- `terraform/modules/auth0` — reusable Auth0 + Azure DevOps resources
- Pipeline and docs live at the project root

## What gets provisioned

| Part | File | Auth0 resource(s) |
| ---- | ---- | ----------------- |
| Management API (provider auth) | `terraform/providers.tf` | `provider "auth0"` |
| Input variables | `terraform/variables.tf` | — |
| Web API (resource server) | `terraform/resource-server.tf` | `auth0_resource_server` |
| API permissions (scopes) | `terraform/permissions.tf` | `auth0_resource_server_scopes` |
| UI client (SPA) | `terraform/client-ui.tf` | `auth0_client` (spa) + `auth0_client_credentials` |
| Web API client (M2M) | `terraform/client-webapi.tf` | `auth0_client` (non_interactive) + `auth0_client_credentials` + `auth0_client_grant` |
| Roles & permission assignment | `terraform/roles.tf` | `auth0_role` + `auth0_role_permissions` |
| Output variable group (Azure DevOps) | `terraform/variable-group.tf` | `azuredevops_variable_group` |
| Outputs | `terraform/outputs.tf` | — |
| Pipeline | `pipeline-auth0.yml` | Azure DevOps CI/CD |

> Provider note (v1.x): inline `scopes` on `auth0_resource_server` and inline `permissions` on `auth0_role` were removed. Scopes are managed with `auth0_resource_server_scopes`, role permissions with `auth0_role_permissions`, and client secrets/auth methods with `auth0_client_credentials`.

## Credential & variable contract

Root Terraform variables are UPPERCASE because `TF_VAR_<NAME>` lookup is case-sensitive (`TF_VAR_AUTH0_CLIENT_SECRET` binds to `variable "AUTH0_CLIENT_SECRET"`).

| Variable | Delivered via | Source |
| -------- | ------------- | ------ |
| `AUTH0_DOMAIN` | `TF_VAR_AUTH0_DOMAIN` | Library group |
| `AUTH0_CLIENT_ID` (sensitive) | `TF_VAR_AUTH0_CLIENT_ID` | Library group (secret) |
| `AUTH0_CLIENT_SECRET` (sensitive) | `TF_VAR_AUTH0_CLIENT_SECRET` | Library group (secret) |
| `ENVIRONMENT` | `TF_VAR_ENVIRONMENT` | Pipeline parameter |
| `API_*`, `UI_*`, `WEBAPI_*`, `ADMIN_ROLE_NAME` | defaults (override with `TF_VAR_*`) | `variables.tf` |

The three `AUTH0_*` credential variables have **no defaults** — nothing sensitive is committed. The Management API application must be an Auth0 **Machine-to-Machine** app authorized for the **Auth0 Management API** with at least: `read:clients`, `create:clients`, `update:clients`, `delete:clients`, `read:client_keys`, `create:client_grants`, `read:resource_servers`, `create:resource_servers`, `update:resource_servers`, `delete:resource_servers`, `read:roles`, `create:roles`, `update:roles`, `delete:roles`.

## State

Terraform state is stored remotely in an **Azure Storage account** blob container (`azurerm` backend, see `terraform/backend.tf`). Only the static, non-secret container coordinates (resource group, storage account, container, key) are committed.

Authentication is handled by the Azure service connection in the pipeline. For local runs, authenticate with `az login` or `ARM_*` environment variables before `terraform init`.

> Prerequisite: the storage account and blob container must exist before the first `terraform init`.

---

## Recommended environment and module structure

For a larger Terraform project, keep each environment as an independent **root module**
and place reusable Auth0 resources in a shared **child module**. This gives `dev` and
`staging` separate state files while both environments use the same resource definitions.

> This is a recommended target structure for this project. It is an example only; the
> current Terraform files remain under `terraform/`.

```text
Auth0-Terraform/
├── environments/
│   ├── dev/
│   │   ├── backend.tf
│   │   ├── main.tf
│   │   ├── outputs.tf
│   │   ├── providers.tf
│   │   └── variables.tf
│   └── staging/
│       ├── backend.tf
│       ├── main.tf
│       ├── outputs.tf
│       ├── providers.tf
│       └── variables.tf
├── modules/
│   └── auth0/
│       ├── client-ui.tf
│       ├── client-webapi.tf
│       ├── outputs.tf
│       ├── permissions.tf
│       ├── resource-server.tf
│       ├── roles.tf
│       ├── variable-group.tf
│       ├── variables.tf
│       └── versions.tf
├── pipeline-auth0.yml
├── master_prompt.md
└── README.md
```

### Responsibilities

- `environments/dev` and `environments/staging` are deployable root modules. Each root
  configures providers, its Azure backend, environment inputs, and the shared module call.
- `modules/auth0` owns all reusable Auth0 and Azure DevOps resources. A child module must
  not configure a backend.
- Each environment uses a different Azure Blob state key so state cannot overlap:
  `auth0/dev.terraform.tfstate` and `auth0/staging.terraform.tfstate`.
- Credentials and environment-specific values continue to come from uppercase
  `TF_VAR_*` environment variables. Do not commit `.tfvars` files or secrets.
- Provider configurations stay in the environment roots and are inherited by the module.
  Provider version constraints belong in both the root and module `versions.tf` or
  `providers.tf` as appropriate.

Example `environments/dev/backend.tf`:

```hcl
terraform {
  backend "azurerm" {
    resource_group_name  = "youtube-examples"
    storage_account_name = "youtube-examples"
    container_name       = "tfbackendyotubenortheu"
    key                  = "auth0/dev.terraform.tfstate"
  }
}
```

Example `environments/staging/backend.tf`:

```hcl
terraform {
  backend "azurerm" {
    resource_group_name  = "youtube-examples"
    storage_account_name = "youtube-examples"
    container_name       = "tfbackendyotubenortheu"
    key                  = "auth0/staging.terraform.tfstate"
  }
}
```

Example environment root module (`environments/dev/main.tf`):

```hcl
module "auth0" {
  source = "../../modules/auth0"

  AUTH0_DOMAIN                   = var.AUTH0_DOMAIN
  AUTH0_CLIENT_ID                = var.AUTH0_CLIENT_ID
  AUTH0_CLIENT_SECRET            = var.AUTH0_CLIENT_SECRET
  ENVIRONMENT                    = var.ENVIRONMENT
  ADO_PROJECT_NAME               = var.ADO_PROJECT_NAME
  OUTPUT_VARIABLE_GROUP_NAME     = var.OUTPUT_VARIABLE_GROUP_NAME
  API_NAME                       = var.API_NAME
  API_IDENTIFIER                 = var.API_IDENTIFIER
  API_SCOPES                     = var.API_SCOPES
  TOKEN_LIFETIME                 = var.TOKEN_LIFETIME
  UI_CLIENT_NAME                 = var.UI_CLIENT_NAME
  UI_CALLBACK_URLS               = var.UI_CALLBACK_URLS
  UI_LOGOUT_URLS                 = var.UI_LOGOUT_URLS
  UI_WEB_ORIGINS                 = var.UI_WEB_ORIGINS
  WEBAPI_CLIENT_NAME             = var.WEBAPI_CLIENT_NAME
  ADMIN_ROLE_NAME                = var.ADMIN_ROLE_NAME
}
```

The staging root uses the same module call. The pipeline supplies different values such
as `TF_VAR_ENVIRONMENT`, `TF_VAR_OUTPUT_VARIABLE_GROUP_NAME`, and deployed UI URLs based
on the selected environment.

Run an environment from its own root directory:

```powershell
cd environments/dev
terraform init
terraform plan
terraform apply
```

Do not run Terraform directly inside `modules/auth0`; child modules are consumed by an
environment root and do not own state.

---

## Master Prompt (Context, Constraints, & Rules)

*Copy and use this Master Prompt when seeding an AI assistant so all generated Auth0 Terraform complies with these standards.*

```markdown
Role: Lead Cloud Identity & IaC Architect (Auth0 + Terraform, 15+ years experience)
Context: Provision an Auth0 tenant with Terraform using the auth0/auth0 provider (~> 1.0). The Auth0 objects back a .NET Web API + Tailwind SPA stack.

Environment & Secrets:
- Management API credentials (domain, client_id, client_secret) are NEVER hardcoded.
- They are supplied via UPPERCASE, case-sensitive TF_VAR_* environment variables:
  TF_VAR_AUTH0_DOMAIN, TF_VAR_AUTH0_CLIENT_ID, TF_VAR_AUTH0_CLIENT_SECRET.
- In Azure DevOps these come from a per-environment Library variable group named
  `auth0-management-api-<env>`; secret variables are explicitly mapped to TF_VAR_* on the task `env:` block.
- The pipeline runs in Azure DevOps and exposes a queue-time `targetEnvironment` choice (dev / staging).

State Backend:
- Store state remotely in an Azure Storage account (backend "azurerm"). Only the container
  coordinates (resource group, storage account, container, key) are committed; authentication
  comes from an Azure service connection (pipeline) or az login / ARM_* env vars (local).

Core Rules & Constraints:
1. Separation of concerns: one file per part — provider/management-api, variables, resource
   server (web api), permissions (scopes), UI client (SPA), Web API client (M2M), roles/permissions, outputs.
2. Provider v1.x compliance:
   - Manage scopes with `auth0_resource_server_scopes` (NOT inline `scopes` on the resource server).
   - Manage role permissions with `auth0_role_permissions` (NOT inline `permissions` on the role).
   - Manage client auth method + secret with `auth0_client_credentials`; read secrets from that resource.
   - Use `scopes` (plural) on `auth0_client_grant`.
3. Data-driven scopes: define API scopes once (var.API_SCOPES) and reuse them for the resource
   server scopes, the M2M client grant, and the admin role permissions via dynamic blocks.
4. Security first: no secrets in code or YAML; all root variables UPPERCASE to match TF_VAR_*;
   sensitive variables and secret outputs flagged `sensitive = true`.
5. Clients: SPA uses Authorization Code + PKCE (authentication_method = "none"); the backend
   M2M client uses client_credentials (authentication_method = "client_secret_post").
6. Naming: suffix every Auth0 object with the environment, e.g. "Products API (dev)".
7. Pipeline: Terraform Install -> Init -> Plan, then a gated Apply behind an Azure DevOps
   Environment (`auth0-<env>`). Never use .tfvars files; inject values through TF_VAR_* env vars.
8. State: store Terraform state remotely in an Azure Storage (azurerm) backend.
```

---

## Run in Azure DevOps

1. Create the Auth0 Management API M2M application (Auth0 Dashboard → Applications → APIs → Auth0 Management API → Machine to Machine Applications) and authorize the scopes listed above.
2. In Azure DevOps → **Pipelines → Library**, create a variable group per environment:
   - `auth0-management-api-dev`
   - `auth0-management-api-staging`
   Add variables `AUTH0_DOMAIN`, `AUTH0_CLIENT_ID`, `AUTH0_CLIENT_SECRET`, marking the client id and secret as **secret** (padlock).
3. Create the Azure DevOps **Environments** for approval gates: `auth0-dev` and `auth0-staging`.
4. Add `pipeline-auth0.yml` as a new pipeline and run it. Pick `targetEnvironment` = `dev` or `staging`.
5. Review the plan, then approve the Apply stage.

## Outputs persisted to an Azure DevOps variable group

The provisioned Auth0 values are written into an Azure DevOps Library variable group **by Terraform itself** (`terraform/variable-group.tf`, `azuredevops_variable_group`) — not by a pipeline script. The group lives in the `YouTube` ADO project (`ADO_PROJECT_NAME`) and `allow_access = true` lets every pipeline in the project consume it.

| Environment | Output variable group (`OUTPUT_VARIABLE_GROUP_NAME`) |
| ----------- | ---------------------------------------------------- |
| `dev` | `dev-auth0-north-eu` |
| `staging` | `staging-auth0-central-eu` |

| Variable | Source | Secret |
| -------- | ------ | ------ |
| `AUTH0_DOMAIN` | `var.AUTH0_DOMAIN` | no |
| `AUTH0_API_AUDIENCE` | `var.API_IDENTIFIER` | no |
| `AUTH0_UI_CLIENT_ID` | `auth0_client.ui.client_id` (web UI / SWA) | no |
| `AUTH0_WEBAPI_CLIENT_ID` | `auth0_client.webapi.client_id` | no |
| `AUTH0_WEBAPI_CLIENT_SECRET` | `auth0_client_credentials.webapi.client_secret` | **yes** (`is_secret`) |

The `azuredevops` provider authenticates with environment variables the pipeline sets on the Terraform Plan/Apply tasks (no secrets in code):

- `AZDO_ORG_SERVICE_URL` = `$(System.CollectionUri)`
- `AZDO_PERSONAL_ACCESS_TOKEN` = `$(System.AccessToken)`

Requirements:

- The pipeline's build service identity must be allowed to **create/manage variable groups** in the `YouTube` project (Library → Security), and **"Allow scripts to access the OAuth token"** must be enabled (default for YAML jobs).
- The variable group is a managed resource, so its variables are reconciled on every apply and Terraform owns their lifecycle. Locally, set `AZDO_ORG_SERVICE_URL` and `AZDO_PERSONAL_ACCESS_TOKEN` (a PAT with Variable Groups read/write) before running.

## Run locally

```powershell
cd terraform

# Authenticate to Azure for the azurerm state backend
az login

terraform init

$env:TF_VAR_ENVIRONMENT         = "dev"
$env:TF_VAR_AUTH0_DOMAIN        = "your-tenant.eu.auth0.com"
$env:TF_VAR_AUTH0_CLIENT_ID     = "xxxxxxxxxxxxxxxxxxxx"
$env:TF_VAR_AUTH0_CLIENT_SECRET = "xxxxxxxxxxxxxxxxxxxx"

# Azure DevOps provider (for the output variable group)
$env:AZDO_ORG_SERVICE_URL       = "https://dev.azure.com/your-org"
$env:AZDO_PERSONAL_ACCESS_TOKEN = "xxxxxxxxxxxxxxxxxxxx"

terraform plan
terraform apply
```

```bash
cd terraform

# Authenticate to Azure for the azurerm state backend
az login

terraform init

export TF_VAR_ENVIRONMENT="dev"
export TF_VAR_AUTH0_DOMAIN="your-tenant.eu.auth0.com"
export TF_VAR_AUTH0_CLIENT_ID="xxxxxxxxxxxxxxxxxxxx"
export TF_VAR_AUTH0_CLIENT_SECRET="xxxxxxxxxxxxxxxxxxxx"

# Azure DevOps provider (for the output variable group)
export AZDO_ORG_SERVICE_URL="https://dev.azure.com/your-org"
export AZDO_PERSONAL_ACCESS_TOKEN="xxxxxxxxxxxxxxxxxxxx"

terraform plan
terraform apply
```

Retrieve outputs (the M2M secret is sensitive):

```bash
terraform output ui_client_id
terraform output webapi_client_id
terraform output -raw webapi_client_secret
```

## Wiring back into the app stack

- `api_identifier` → the Web API `Auth0__Audience` and the SPA `auth0Config.audience`.
- `ui_client_id` → the SPA `auth0Config.clientId` in the frontend `app.js`.
- Update `UI_CALLBACK_URLS`, `UI_LOGOUT_URLS`, and `UI_WEB_ORIGINS` to include your deployed frontend URL(s).
