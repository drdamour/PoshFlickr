

using FlickrNetCore.Resources;

namespace FlickrNetCore.Responses;

public record AlbumResponse(
	[property: JsonPropertyName("photoset")]
	AlbumResource Album,
    string Stat,
	decimal Code,
	string Message
) : BaseResponse(
	Stat,
	Code,
	Message
);


