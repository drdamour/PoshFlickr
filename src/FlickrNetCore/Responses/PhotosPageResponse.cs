using FlickrNetCore.Resources;

namespace FlickrNetCore.Responses;

public record PhotosPageResponse(
	[property: JsonPropertyName("photoset")]
	PhotoPageResource Page,
    string Stat,
	decimal Code,
	string Message
) : BaseResponse(
	Stat,
	Code,
	Message
);


