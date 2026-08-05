# ==========================================
# REMOTE STATE BACKEND (Azure Storage)
# ==========================================
# Terraform state is stored in an Azure Storage account blob container instead of
# local disk. The values below match what the Azure DevOps pipeline passes at
# `terraform init` time; authentication is handled by the Azure service
# connection (pipeline) or `az login` / ARM_* environment variables (local).
terraform {
  backend "azurerm" {
    resource_group_name  = "youtube-examples"
    storage_account_name = "youtube-examples"
    container_name       = "tfbackendyotubenortheu"
    key                  = "auth0-managment"
  }
}
