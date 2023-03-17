using System.Text.Json;

namespace FlickrNetCore.Serialization;

//IsThing : 1 => true
public class NumberAcceptingBoolConverter : JsonConverter<bool>
{
    public override bool Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    { 

        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.TryGetInt64(out var i) && i > 0;
        }

        return reader.GetBoolean();

    }


    public override void Write(
        Utf8JsonWriter writer,
        bool value,
        JsonSerializerOptions options
    )
    {
        writer.WriteBooleanValue(value);
    }
}

