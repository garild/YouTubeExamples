

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var signingKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF"));//Weak

var encryptionKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes("1234567890ABCDEF1234567890ABCDEF")); //Weak

// Default dummy claims for demonstration purposes
var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, "TestId-1234"),
    new Claim(ClaimTypes.Email, "admin@company.com"),
    new Claim(ClaimTypes.Role, "Administrator"),
    new Claim("salary", "$250000")
};

var token = new JwtSecurityToken(

    issuer: "DemoApi",
    audience: "DemoClient",
    claims: claims,
    expires: DateTime.UtcNow.AddMinutes(30),

        signingCredentials: new SigningCredentials(
        signingKey,
        SecurityAlgorithms.HmacSha256)

);

var encryptingCredentials = new EncryptingCredentials(
        encryptionKey,
        SecurityAlgorithms.Aes256KW,
        SecurityAlgorithms.Aes256CbcHmacSha512);

var handler = new JwtSecurityTokenHandler();

var jwt = handler.WriteToken(token);


// readable JWT (JWS)
Console.WriteLine(jwt);

var jsonWebToken = new JsonWebTokenHandler();
string jweToken = jsonWebToken.EncryptToken(jwt, encryptingCredentials);


Console.WriteLine("Encrypted JWT (JWE)");
Console.WriteLine(jweToken);


// Read the token back
var validationParameters = new TokenValidationParameters
{
    ValidIssuer = "DemoApi",
    ValidAudience = "DemoClient",

    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,

    IssuerSigningKey = signingKey,
    TokenDecryptionKey = encryptionKey
};

// JWT is my token which I have created and encrypted above
handler.ValidateToken(
    jwt,
    validationParameters,
    out var validatedToken);

Console.WriteLine("Successfully decrypted!");

var decrypted = (JwtSecurityToken)validatedToken;

Console.WriteLine($"Subject: {decrypted.Issuer}");




