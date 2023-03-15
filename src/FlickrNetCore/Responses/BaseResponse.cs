
namespace FlickrNetCore.Responses;

//flickr returns data requests in an envelope like this
public abstract record BaseResponse(
    string Stat,
	decimal Code,
	string Message
);


