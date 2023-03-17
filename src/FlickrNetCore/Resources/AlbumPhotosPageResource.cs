namespace FlickrNetCore.Resources;

public record AlbumPhotosPageResource(
    string Id,
    decimal Page,
    [property: JsonPropertyName("pages")]
    decimal TotalPages,
    [property: JsonPropertyName("perpage")]
    decimal PhotosPerPage,
    [property: JsonPropertyName("total")]
    decimal TotalPhotos,
    //TODO: there's a bunch of stuff in response equivalent to AlbumResource on this we could map

    [property: JsonPropertyName("photo")]
    IEnumerable<AlbumPhotoResource> Photos
);

