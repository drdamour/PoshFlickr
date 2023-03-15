using Flurl;

namespace FlickrNetCore.Auth;

public record AccessToken(
	string Value,
	string Secret,
	string UserName,
	string UserId,
	string UserFullName
) : OAuthToken(
    Value,
    Secret
);


