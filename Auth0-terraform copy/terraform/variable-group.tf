# ==========================================
# AZURE DEVOPS OUTPUT VARIABLE GROUP
# ==========================================
# Persists the provisioned Auth0 values into an Azure DevOps Library variable group
# so the downstream app pipelines can consume them. Managed entirely by Terraform.
data "azuredevops_project" "project" {
  name = var.ADO_PROJECT_NAME
}

resource "azuredevops_variable_group" "auth0_outputs" {
  project_id   = data.azuredevops_project.project.id
  name         = var.OUTPUT_VARIABLE_GROUP_NAME
  description  = "Auth0 outputs (managed by Terraform)"
  allow_access = true # Allow all pipelines in the project to use it

  variable {
    name  = "AUTH0_DOMAIN"
    value = var.AUTH0_DOMAIN
  }

  variable {
    name  = "AUTH0_API_AUDIENCE"
    value = var.API_IDENTIFIER
  }

  variable {
    name  = "AUTH0_UI_CLIENT_ID"
    value = auth0_client.ui.client_id
  }

  variable {
    name  = "AUTH0_WEBAPI_CLIENT_ID"
    value = auth0_client.webapi.client_id
  }

  variable {
    name         = "AUTH0_WEBAPI_CLIENT_SECRET"
    secret_value = auth0_client_credentials.webapi.client_secret
    is_secret    = true
  }
}
