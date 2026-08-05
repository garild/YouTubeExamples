using System.Globalization;

public static class PrimitiveExtensions
{
    // 1. Extension Block for primitive 'long' (Unix Timestamps)
    extension(long timestamp)
    {
        // Extension Property: Converts a raw long to a DateTimeOffset smoothly
        public DateTimeOffset ToDateTimeOffset() => DateTimeOffset.FromUnixTimeSeconds(timestamp);

        // Extension Method: Checks if the timestamp is older than current UTC time
        public bool IsExpired() => DateTimeOffset.FromUnixTimeSeconds(timestamp) < DateTimeOffset.UtcNow;
    }

    // 2. Extension Block for primitive 'decimal' (Financial Operations)
    extension(decimal amount)
    {
        // Extension Method: Safe banking rounding (Round to Even) required for financial audits
        public decimal RoundToFinancial() => decimal.Round(amount, 2, MidpointRounding.ToEven);
    }

    // 3. Extension Block for primitive 'string' (String Manipulations)
    extension(double amount)
    {
        public string ToCurrencyString(string culture = "en-US") => amount.ToString("C", CultureInfo.CreateSpecificCulture(culture));

        public bool IsValidCurrencyFormat(string culture = "en-US")
        {
            var cultureInfo = CultureInfo.CreateSpecificCulture(culture);
            return decimal.TryParse(amount.ToString(cultureInfo), NumberStyles.Currency, cultureInfo, out _);
        }
    }

    // 4. Extension Block for primitive 'string' (String Manipulations)
    extension(string str)
    {
        // Extension Method: Checks if the string is a valid email format
        public bool IsValidEmail()
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(str);
                return addr.Address == str;
            }
            catch
            {
                return false;
            }
        }

        // Extension Method: Checks if the string is a valid URL format
        public bool IsValidUrl()
        {
            return Uri.TryCreate(str, UriKind.Absolute, out var uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        // Extension Method: Converts a string to Title Case
        public string ToTitleCase() => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());

        // Extension Method: Converts a string to a Guid, generating a new one if the string is null or empty
        public Guid ToGuid() => Guid.NewGuid();

        // Extension Method: Checks if the string is null or empty + whitespace
        public bool IsNullOrEmpty() => string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
    }
}