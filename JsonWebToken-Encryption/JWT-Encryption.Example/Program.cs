
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;


var readableJWT = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6Ik52RmxWUkluRklhaFlMU0l3bHVxbCJ9.eyJpc3MiOiJodHRwczovL2Rldi15b3V0dWJlLWd0LmV1LmF1dGgwLmNvbS8iLCJzdWIiOiJUSXRkaVZpN1JIY3ZRb2JDMXYyYkNQQnFEN2ZBMlo5cUBjbGllbnRzIiwiYXVkIjoiaHR0cHM6Ly9hcGkucHJvZHVjdHMubG9jYWwiLCJpYXQiOjE3ODQzNzMxODUsImV4cCI6MTc4NDQ1OTU4NSwic2NvcGUiOiJyZWFkOnByb2R1Y3RzIHdyaXRlOnByb2R1Y3RzIiwiZ3R5IjoiY2xpZW50LWNyZWRlbnRpYWxzIiwiYXpwIjoiVEl0ZGlWaTdSSGN2UW9iQzF2MmJDUEJxRDdmQTJaOXEifQ.tmZqUs2awQs1kxyTvXIyjGN1I-9OieyDUVFnGlkToeL8L5tqymMvFmcGFtXnqLyqU5mlz84gm-Dk2_r84nFNSVFXsxRkdhVpENoNlTvhdrEom7StE9xVRVRH8Pj2FPm_y6mPGudxrTpdN_QrAQSkA8PNXaKk8dNKV8gX2_O8SdBfN6LB9H3AcDkRutaoS5NyBIOJ_lUowXcYrrrjlHLDnFbQEnATW_cdIQ_x29i6fg1qmZHBnva_0jX1DDxY0mHg33hm9BoxRcfB8Bxui7AlrQRcNEdf65dETvPTRktA2-aFIdE_FboQeHW4UuyI_L_XK3HJw2fGaiB6GY5X_i2nAg";

using var rsa = RSA.Create();

var encryptingCredentials = new EncryptingCredentials(
    new RsaSecurityKey(rsa),
    SecurityAlgorithms.RsaOAEP,
    SecurityAlgorithms.Aes256CbcHmacSha512);

var descriptor = new SecurityTokenDescriptor
{
    Claims = new Dictionary<string, object>
    {
        ["sub"] = "auth0|123456|SomeId",
        ["name"] = "Garib"
    },
    Audience = "https://api.products.local",
    EncryptingCredentials = encryptingCredentials
};

var handler = new JsonWebTokenHandler();

var jwe = handler.CreateToken(descriptor);

var encrypting = handler.EncryptToken(readableJWT, encryptingCredentials);
Console.WriteLine(encrypting);
Console.ReadKey();