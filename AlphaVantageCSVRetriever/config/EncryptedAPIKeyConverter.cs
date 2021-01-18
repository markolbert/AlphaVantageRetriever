using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public class EncryptedAPIKeyConverter : JsonConverter<Configuration>
    {
        public override Configuration? Read( ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options )
        {
            throw new NotImplementedException( "This converter cannot be used for deserializing AppConfig objects" );
        }

        public override void Write( Utf8JsonWriter writer, Configuration value, JsonSerializerOptions options )
        {
            // we only want to serialize the APIKeys property
            writer.WriteStartObject();
            writer.WriteString( "EncyrptedAPIKey", value.EncyrptedAPIKey );
            writer.WriteEndObject();
        }
    }
}