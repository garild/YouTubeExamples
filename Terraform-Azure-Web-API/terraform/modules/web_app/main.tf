resource "azurerm_service_plan" "app_plan" {
  name                = "plan-${var.project_name}-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  os_type             = "Linux"
  sku_name            = "B1"
}

resource "azurerm_linux_web_app" "web_api" {
  name                = "app-${var.project_name}-api-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  service_plan_id     = azurerm_service_plan.app_plan.id

  site_config {
    application_stack {
      dotnet_version = "8.0"
    }
    cors {
      allowed_origins     = ["*"]
      support_credentials = false
    }
  }

  app_settings = {
    "Auth0__Domain"          = var.auth0_domain
    "Auth0__Audience"        = var.auth0_audience
    "ASPNETCORE_ENVIRONMENT" = "Development"
  }
}

resource "azurerm_static_web_app" "web_ui" {
  name                = "swa-${var.project_name}-ui-${var.environment}"
  location            = "eastus2" # Static Web Apps restricted deployment regions
  resource_group_name = var.resource_group_name
  sku_tier            = "Free"
  sku_size            = "Free"
}
