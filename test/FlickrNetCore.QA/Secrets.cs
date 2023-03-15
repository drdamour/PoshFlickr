using System;
using FlickrNetCore.Auth;

namespace FlickrNetCore.QA;

public static class Secrets
{
	
	public const string RedactedMarker = "[REDACTED]";

	public static string VerifyAPISecretSet()
	{
        //HACK: Unclear what the right way to inject these for QA test is
        string APISecret = "[REDACTED]";

        if (APISecret == RedactedMarker)
		{
			throw new Exception("This test requires a real APISecret value set");
		}

		return APISecret;
	}

    public static AccessToken VerifyAccessTokenSet()
    {
        //HACK: Unclear what the right way to inject these for QA test is
        AccessToken token = new AccessToken(
            "72157720876237838-8794db21460b85f8",
            "[REDACTED]",
            "ce_doit_etre",
            "63505810@N00",
            ""
        );

        if (token.Secret == RedactedMarker)
        {
            throw new Exception("This test requires a real AccessToken value set");
        }

        return token;
    }

}

