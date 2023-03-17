using Flurl;

namespace FlickrNetCore.Auth;

//TODO: add expirey info if possible
public record RequestToken(
	string Value,
	string Secret
) : OAuthToken(
    Value,
    Secret
)
{
    private const string authorizeBaseHref = "https://www.flickr.com/services/oauth/authorize";

    public string MakeAuthorizeHref(
        AuthLevel permissionLevel
    )
    {

        return authorizeBaseHref
            .SetQueryParam(
                "oauth_token",
                this.Value
            )
            .SetQueryParam(
                "perms",
                permissionLevel.ToString().ToLower()
            );
    }
};


