
namespace FlickrNetCore.Responses;

public abstract record BaseResponse(
    string Stat,
	decimal Code,
	string Message
);


