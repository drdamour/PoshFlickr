using Flurl;

namespace FlickrNetCore.Auth;

public abstract record OAuthToken(
	string Value,
	string Secret
);



