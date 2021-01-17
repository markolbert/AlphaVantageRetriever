using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using J4JSoftware.ConsoleUtilities;

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

        [Updater(typeof(OutputFileUpdater))]
        public string OutputFilePath { get; set; }

        [Updater(typeof(CallsPerMinuteUpdater))]
        public float CallsPerMinute { get; set; } = 4.5F;

        [Updater(typeof(TickerUpdater))]
        public List<string> Tickers { get; set; } = new();

        public bool EncryptKey { get; set; }

        [ JsonIgnore ]
        public string ApiKey
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
        public string ApiKeyEncrypted
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

        [JsonPropertyName("ApiKey")]
        private string ApiKeyHidden
        {
            set => ApiKeyEncrypted = value;
        }
    }
}
