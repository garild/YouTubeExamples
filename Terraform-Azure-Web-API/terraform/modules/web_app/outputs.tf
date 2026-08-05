output "api_url" {
  value = "https://${azurerm_linux_web_app.web_api.default_hostname}"
}

output "ui_url" {
  value = "https://${azurerm_static_web_app.web_ui.default_host_name}"
}

output "swa_deployment_token" {
  value     = azurerm_static_web_app.web_ui.api_key
  sensitive = true
}
