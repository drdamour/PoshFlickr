using System.Text.Json;

namespace FlickrNetCore.Serialization;

//turns "x y" to ["x","y"]
public class SpaceDelimittedStringCollectionConverter : JsonConverter<IEnumerable<string>>
{
    public override IEnumerable<string>? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    { 

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("expected string");
        }

        return reader.GetString()?.Split(" ", StringSplitOptions.RemoveEmptyEntries);

    }


    public override void Write(
        Utf8JsonWriter writer,
        IEnumerable<string> value,
        JsonSerializerOptions options
    )
    {
        throw new NotImplementedException();
    }
}

