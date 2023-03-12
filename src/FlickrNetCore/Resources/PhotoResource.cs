using System;

namespace FlickrNetCore.Resources;

public record PhotoResource(
    string Id,
    string Title,
    string Description
);

