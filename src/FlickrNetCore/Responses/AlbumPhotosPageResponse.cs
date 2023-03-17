using FlickrNetCore.Resources;

namespace FlickrNetCore.Responses;

public record AlbumPhotosPageResponse(
	[property: JsonPropertyName("photoset")]
	AlbumPhotosPageResource Page,
    string Stat,
	decimal Code,
	string Message
) : BaseResponse(
	Stat,
	Code,
	Message
);


