using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlickrNetCore.Serialization;

//for whatever dumb reason flickr returns some strings like
//{
//  "stringName" : {
//     "_content" : "string value"
//  }
//
public class StringContentConverter : JsonConverter<string>
{
    public override string? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        //sometimes, title IS just a string, like when it's in a photoset result
        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("expected start object");
        }

        while(reader.Read() && reader.TokenType != JsonTokenType.EndObject )
        {
            if(reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "_content")
            {
                //technically i should loop here looking for a prop val token and not unexpected things
                //in case a comment is present or soemthing really odd
                if(reader.Read() && reader.TokenType == JsonTokenType.String)
                {
                    var val = reader.GetString();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                    {
                        //read till the object is closed
                    }
                    return val;
                    
                }
            }
        }

        throw new JsonException("expected to find a property _content but never did");

    }


    public override void Write(
        Utf8JsonWriter writer,
        string value,
        JsonSerializerOptions options
    )
    {
        throw new NotImplementedException();
    }
}

