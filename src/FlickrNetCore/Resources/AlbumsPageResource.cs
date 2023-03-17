namespace FlickrNetCore.Resources;

public record AlbumsPageResource(
    decimal Page,
    [property: JsonPropertyName("pages")]
    decimal TotalPages,
    [property: JsonPropertyName("perpage")]
    decimal AlbumsPerPage,
    [property: JsonPropertyName("total")]
    decimal TotalAlbums,
    //TODO: switch to bool via json convert magic
    int CanCreate,
    [property: JsonPropertyName("photoset")]
    IEnumerable<AlbumResource> Albums
);

