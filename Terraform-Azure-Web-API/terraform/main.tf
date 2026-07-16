resource "azurerm_resource_group" "rg" {
  name     = "rg-${var.PROJECT_NAME}-${var.ENVIRONMENT}"
  location = var.LOCATION
}

module "storage" {
  source              = "./modules/storage"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  project_name        = var.PROJECT_NAME
  environment         = var.ENVIRONMENT
}

module "service_bus" {
  source              = "./modules/service_bus"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  project_name        = var.PROJECT_NAME
  environment         = var.ENVIRONMENT
}

module "key_vault" {
  source              = "./modules/key_vault"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  project_name        = var.PROJECT_NAME
  environment         = var.ENVIRONMENT
}

module "web_app" {
  source              = "./modules/web_app"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  project_name        = var.PROJECT_NAME
  environment         = var.ENVIRONMENT
  auth0_domain        = var.AUTH0_DOMAIN
  auth0_audience      = var.AUTH0_AUDIENCE
}
