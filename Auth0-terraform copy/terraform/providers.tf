# ==========================================
# PROVIDER CONFIGURATION (Auth0 Management API)
# ==========================================
# The Management API credentials are NEVER hardcoded. They are supplied at run time
# through UPPERCASE TF_VAR_* environment variables (TF_VAR_AUTH0_DOMAIN,
# TF_VAR_AUTH0_CLIENT_ID, TF_VAR_AUTH0_CLIENT_SECRET), which the Azure DevOps
# pipeline maps from a Library variable group.
terraform {
  required_version = ">= 1.5.0"
  required_providers {
    auth0 = {
      source  = "auth0/auth0"
      version = "~> 1.0"
    }
    azuredevops = {
      source  = "microsoft/azuredevops"
      version = "~> 1.0"
    }
  }
}

provider "auth0" {
  domain        = var.AUTH0_DOMAIN
  client_id     = var.AUTH0_CLIENT_ID
  client_secret = var.AUTH0_CLIENT_SECRET
}

# Azure DevOps provider. Authentication is supplied via environment variables set by
# the pipeline (no secrets in code):
#   AZDO_ORG_SERVICE_URL       = $(System.CollectionUri)
#   AZDO_PERSONAL_ACCESS_TOKEN = $(System.AccessToken)
provider "azuredevops" {
}
