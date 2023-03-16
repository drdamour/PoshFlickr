﻿namespace FlickrNetCore.Resources;

//TODO: maybe this should be AlbumPhotoPage cause the photos are slightly different...
public record PhotoPageResource(
    decimal Page,
    [property: JsonPropertyName("pages")]
    decimal TotalPages,
    [property: JsonPropertyName("perpage")]
    decimal PhotosPerPage,
    [property: JsonPropertyName("total")]
    decimal TotalPhotos,
    //TODO: there's a bunch of stuff in response equivalent to AlbumResource on this we could map

    [property: JsonPropertyName("photo")]
    IEnumerable<PhotoResource> Photos
);
