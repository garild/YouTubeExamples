
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;


var tokenDescriptor = """
    eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6Ik52RmxWUkluRklhaFlMU0l3bHVxbCJ9.
    eyJpc3MiOiJodHRwczovL2Rldi15b3V0dWJlLWd0LmV1LmF1dGgwLmNvbS8iLCJzdWIiOiJUSXRka
    VZpN1JIY3ZRb2JDMXYyYkNQQnFEN2ZBMlo5cUBjbGllbnRzIiwiYXVkIjoiaHR0cHM6Ly9hcGkucH
    JvZHVjdHMubG9jYWwiLCJpYXQiOjE3ODQzNzMxODUsImV4cCI6MTc4NDQ1OTU4NSwic2NvcGUiOiJ
    yZWFkOnByb2R1Y3RzIHdyaXRlOnByb2R1Y3RzIiwiZ3R5IjoiY2xpZW50LWNyZWRlbnRpYWxzIiwi
    YXpwIjoiVEl0ZGlWaTdSSGN2UW9iQzF2MmJDUEJxRDdmQTJaOXEifQ.
    tmZqUs2awQs1kxyTvXIyjGN1I-9OieyDUVFnGlkToeL8L5tqymMvFmcGFtXnqLyqU5mlz84gm-Dk2
    _r84nFNSVFXsxRkdhVpENoNlTvhdrEom7StE9xVRVRH8Pj2FPm_y6mPGudxrTpdN_QrAQSkA8PNXaKk8dNKV8gX2_
    O8SdBfN6LB9H3AcDkRutaoS5NyBIOJ_lUowXcYrrrjlHLDnFbQEnATW_cdIQ_x29i6fg1qmZHBnva_
    0jX1DDxY0mHg33hm9BoxRcfB8Bxui7AlrQRcNEdf65dETvPTRktA2-aFIdE_FboQeHW4UuyI_L_
    XK3HJw2fGaiB6GY5X_i2nAg
    """;

// 1. RSA key pair
//    In production: load from Azure Key Vault or X.509 certificate
using var rsa = RSA.Create(2048);
var rsaSecurityKey = new RsaSecurityKey(rsa)
{
    KeyId = "rsa-key-2026"
};

// 2. Encryption credentials
//    Key wrap algorithm:      RSA-OAEP        (wraps the symmetric CEK)
//    Content encryption algo: AES-256-CBC + HMAC-SHA-512
var encryptingCredentials = new EncryptingCredentials(
    rsaSecurityKey,
    SecurityAlgorithms.RsaOAEP,
    SecurityAlgorithms.Aes256CbcHmacSha512);

// 3. Token descriptor — notice NO SigningCredentials needed

// 4. Create the JWE token
var handler = new JsonWebTokenHandler();
string jweToken = handler.EncryptToken(tokenDescriptor, encryptingCredentials);

// Output structure: 5 base64url parts separated by dots
// header . encryptedKey . iv . ciphertext . authTag
//
// Decode header:  {"alg":"RSA-OAEP","enc":"A256CBC-HS512","typ":"JWT"}
// Everything else: encrypted — unreadable without the RSA private key
Console.WriteLine(jweToken);