﻿using FlickrNetCore.Resources;

namespace FlickrNetCore.Responses;

public record AlbumsPageResponse(
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


