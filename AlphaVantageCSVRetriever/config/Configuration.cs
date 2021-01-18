using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public partial class Configuration
    {
        private string _encryptedKey = string.Empty;
        private string _key = string.Empty;

        public Configuration()
        {
            OutputFilePath = GetDefaultOutputFile();
        }

        public string OutputFilePath { get; set; }
        public float CallsPerMinute { get; set; } = 4.5F;
        public List<string> Tickers { get; set; } = new();

        public bool EncryptKey { get; set; }

        [ JsonIgnore ]
        public string APIKey
        {
            get => _key;

            set
            {
                _key = value;

                // no point encrypting empty or null strings
                if( string.IsNullOrEmpty( _key ) )
                    return;

                if( CompositionRoot.Default.Protect( _key, out var encrypted ) )
                    _encryptedKey = encrypted!;
            }
        }

        [ JsonIgnore ]
        public string EncyrptedAPIKey
        {
            get => _encryptedKey;

            set
            {
                _encryptedKey = value;

                // no point decrypting empty or null strings
                if( string.IsNullOrEmpty( _encryptedKey ) )
                    return;

                if( CompositionRoot.Default.Unprotect( _encryptedKey, out var decrypted ) )
                    _key = decrypted!;
            }
        }

        [ JsonPropertyName( "APIKey" ) ]
        // ReSharper disable once UnusedMember.Local
        private string ApiKeyHidden
        {
            set => EncyrptedAPIKey = value;
        }
    }
}