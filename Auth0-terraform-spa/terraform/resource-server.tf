# ==========================================
# WEB API (RESOURCE SERVER)
# ==========================================
# Represents the protected .NET Web API. The `identifier` is the audience that the
# API validates on incoming JWTs. Scopes are managed separately in permissions.tf.
resource "auth0_resource_server" "products_api" {
  name        = "${var.API_NAME} (${var.ENVIRONMENT})"
  identifier  = var.API_IDENTIFIER
  signing_alg = "RS256"

  allow_offline_access                            = true
  token_lifetime                                  = var.TOKEN_LIFETIME
  skip_consent_for_verifiable_first_party_clients = true
}
