using FlickrNetCore.Resources;

namespace FlickrNetCore.Responses;

public record PhotosetsPageResponse(
	[property: JsonPropertyName("photosets")]
	AlbumPageResource Page,
    string Stat,
	decimal Code,
	string Message
) : BaseResponse(
	Stat,
	Code,
	Message
);


