using System;
using System.Text.Json.Serialization;
using FlickrNetCore.Serialization;

namespace FlickrNetCore.Resources;

//TODO: don't use JsonConvert, instead tell serializer how to handle "_content" like props
//that way the record can serialize to json normally
//or have stirng content converter be smart and work with name:val as well
public record PhotoResource(
    string Id,
    [property: JsonConverter(typeof(StringContentConverter))]
    string Title,
    [property: JsonConverter(typeof(StringContentConverter))]
    string Description
);

