using FlickrNetCore.Serialization;

namespace FlickrNetCore.Resources;

//TODO: don't use JsonConvert, instead tell serializer how to handle "_content" like props
//that way the record can serialize to json normally
//or have stirng content converter be smart and work with name:val as well
public record AlbumPhotoResource(
    string Id,
    string AlbumId,
    string Title,
    [property: JsonConverter(typeof(NumberAcceptingBoolConverter))]
    bool IsPublic,
    //TODO: these can be null since they are only filled when included in extras[
    [property: JsonConverter(typeof(SpaceDelimittedStringCollectionConverter))]
    IEnumerable<string>? Tags
    //TODO: support other extras props
);

