
using FlickrNetCore.Resources;

namespace FlickrNetCore.Responses;

public record PhotoResponse(
	PhotoResource Photo,
    string Stat,
	decimal Code,
	string Message
) : BaseResponse(
	Stat,
	Code,
	Message
);


