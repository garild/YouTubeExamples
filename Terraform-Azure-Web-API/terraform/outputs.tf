output "resource_group_name" {
  value       = azurerm_resource_group.rg.name
  description = "The name of the provisioned Resource Group."
}

output "api_url" {
  value       = module.web_app.api_url
  description = "The root URL of the deployed .NET Web API."
}

output "ui_url" {
  value       = module.web_app.ui_url
  description = "The root URL of the deployed Static Web App frontend."
}

output "swa_deployment_token" {
  value       = module.web_app.swa_deployment_token
  description = "The deployment token used to publish Static Web App frontend content."
  sensitive   = true
}

output "key_vault_uri" {
  value       = module.key_vault.key_vault_uri
  description = "The URI of the created Azure Key Vault."
}

output "storage_account_name" {
  value       = module.storage.storage_account_name
  description = "The name of the created Azure Storage Account (S3 equivalent)."
}

output "service_bus_topic_name" {
  value       = module.service_bus.topic_name
  description = "The name of the Service Bus Topic created."
}
