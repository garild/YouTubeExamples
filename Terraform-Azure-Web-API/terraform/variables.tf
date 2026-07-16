variable "PROJECT_NAME" {
  type        = string
  description = "The name of the project."
  default     = "yt-examples"
}

variable "ENVIRONMENT" {
  type        = string
  description = "Target deployment environment (e.g. dev, staging). Supply via TF_VAR_ENVIRONMENT."
  default     = "dev"
}

variable "LOCATION" {
  type        = string
  description = "Azure region where all resources will be provisioned (dev: northeurope, staging: westeurope). Supply via TF_VAR_LOCATION."
  default     = "northeurope"
}

variable "SUBSCRIPTION_ID" {
  type        = string
  description = "Azure Subscription ID for deployment."
  default     = "2a0fd781-da04-4038-9095-7b677ae5986a"
}

variable "AUTH0_DOMAIN" {
  type        = string
  description = "The domain name of the Auth0 Tenant. Supply via TF_VAR_AUTH0_DOMAIN."
  default     = "dev-tenant.auth0.com"
}

variable "AUTH0_AUDIENCE" {
  type        = string
  description = "The unique identifier audience for the registered Auth0 Web API. Supply via TF_VAR_AUTH0_AUDIENCE."
  default     = "https://api.products.local"
}
