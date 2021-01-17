using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AlphaVantageKey
{
    public class EncryptedData
    {
#pragma warning disable 8618
        public string ApiKeyEncrypted { get; set; }
#pragma warning restore 8618
    }

    class Program
    {
        private const string KeyFile = "AlphaVantageAPI.key";

        static void Main(string[] args)
        {
            Console.Write("Enter AlphaVantage API key (blank to skip): ");
            var apiKey = Console.ReadLine();

            if( string.IsNullOrEmpty( apiKey ) )
                return;

            var bytes = Encoding.Unicode.GetBytes( apiKey );
#pragma warning disable CA1416 // Validate platform compatibility
            var encoded = ProtectedData.Protect( bytes, null, DataProtectionScope.CurrentUser );
#pragma warning restore CA1416 // Validate platform compatibility

            var encrypted = new EncryptedData()
            {
                ApiKeyEncrypted = Convert.ToBase64String( encoded )
            };

            if( File.Exists(KeyFile))
                File.Delete(KeyFile);

            File.WriteAllText( KeyFile, JsonSerializer.Serialize<EncryptedData>( encrypted ) );
        }
    }
}
