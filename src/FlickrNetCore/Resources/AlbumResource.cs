using System;
using FlickrNetCore.Serialization;

namespace FlickrNetCore.Resources;

public record AlbumResource(
    [property: JsonPropertyName("owner")]
    string OwnerId,
    string Id,
    [property: JsonConverter(typeof(StringContentConverter))]
    string Title,
    [property: JsonConverter(typeof(StringContentConverter))]
    string Description
);

