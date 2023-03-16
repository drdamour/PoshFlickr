using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using FlickrNetCore.Auth;
using FlickrNetCore.Resources;
using Flurl;

namespace FlickrNetCore;

public partial class FlickrClient
{
    /// <summary>
    /// a callback href used to tell oauth process to display the verify code to the user instead of passing it around 
    /// </summary>
    public const string OUT_OF_BAND_CALLBACK_HREF = "oob";

    private const string baseHref = "https://api.flickr.com/services/rest?format=json&nojsoncallback=1";
    private const string requestTokenBaseHref = "https://www.flickr.com/services/oauth/request_token";
    private const string accessTokenBaseHref = "https://www.flickr.com/services/oauth/access_token";

    private readonly HttpClient httpClient;
    private readonly Options options;

    

    public FlickrClient(
        HttpClient httpClient,
        Options options
    )
    {
        this.httpClient = httpClient;
        this.options = options;
        this.Photos = new PhotoClient(this);
        this.Albums = new AlbumClient(this);
    }

    public Task<RequestToken> FetchRequestToken(        
        CancellationToken cancellationToken
    )
    {
        return this.FetchRequestToken(
            OUT_OF_BAND_CALLBACK_HREF,
            cancellationToken:cancellationToken
        );
    }

    public async Task<RequestToken> FetchRequestToken(
        string callbackHref = OUT_OF_BAND_CALLBACK_HREF,
        CancellationToken cancellationToken = default
    )
    {

        var result = await httpClient.GetAsync(
            MakeOAuthUrl(
                new Flurl.Url(requestTokenBaseHref)
                    .SetQueryParam(
                        "oauth_callback",
                        callbackHref
                    )
            ),
            cancellationToken
        );

        //TODO: check response code

        //gives back results like oauth_callback_confirmed=true&oauth_token=72157720876234118-e29fba0da96d4311&oauth_token_secret=6310da33c73fa37a
        var resultParamsByName = HttpUtility.ParseQueryString(
            await result.Content.ReadAsStringAsync(cancellationToken)
        );

        if (resultParamsByName["oauth_callback_confirmed"] == "true")
        {
            return new RequestToken(
                resultParamsByName["oauth_token"] ?? throw new Exception("no oauth_token in request token response"),
                resultParamsByName["oauth_token_secret"] ?? throw new Exception("no oauth_token_secret in request token response")
            );
        }

        //TODO: there's data in the response we can include
        throw new Exception("request token fetch failed");
       
    }


    public async Task<AccessToken> FetchAccessToken(
        RequestToken requestToken,
        string verifyToken,
        CancellationToken cancellationToken = default
    )
    {

        var result = await httpClient.GetAsync(
            MakeOAuthUrl(
                new Flurl.Url(accessTokenBaseHref)
                    .SetQueryParam(
                        "oauth_verifier",
                        verifyToken
                    ),
                requestToken
            ),
            cancellationToken
        );

        //TODO: check response code

        //gives back results like oauth_callback_confirmed=true&oauth_token=72157720876234118-e29fba0da96d4311&oauth_token_secret=6310da33c73fa37a
        var resultParamsByName = HttpUtility.ParseQueryString(
            await result.Content.ReadAsStringAsync(cancellationToken)
        );

        if (resultParamsByName["oauth_token"] != "" && resultParamsByName["oauth_token_secret"] != "")
        {
            return new AccessToken(
                resultParamsByName["oauth_token"] ?? throw new Exception("no oauth_token in request token response"),
                resultParamsByName["oauth_token_secret"] ?? throw new Exception("no oauth_token_secret in request token response"),
                resultParamsByName["username"] ?? throw new Exception("no username in request token response"),
                resultParamsByName["user_nsid"] ?? throw new Exception("no user_nsid in request token response"),
                resultParamsByName["fullname"] ?? throw new Exception("no fullname in request token response")
            );
        }

        //TODO: there's data in the response we can include
        throw new Exception("access token fetch failed");

    }



    internal Flurl.Url MakeOAuthUrl(
        Flurl.Url url,
        OAuthToken? token = null,
        HttpMethod? method = null    
    )
    {
        url = url
            .Clone()
            .SetQueryParam(
                "oauth_consumer_key",
               this.options.APIKey
            )
            .SetQueryParam(
                "oauth_nonce",
                Random.Shared.Next(100, 99999)
            )
            .SetQueryParam(
                "oauth_signature_method",
                "HMAC-SHA1"
            )
            .SetQueryParam(
                "oauth_version",
                "1.0"
            )
            .SetQueryParam(
                "oauth_timestamp",
                DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            )
            ;

        if(token != null)
        {
            url.SetQueryParam(
                "oauth_token",
                token.Value
            );
        }
            

        string baseStr = method?.Method ?? "GET"
            + "&"
            + Uri.EscapeDataString(
                url.Clone().RemoveQuery()
            )
            + "&"
            + Uri.EscapeDataString(
                string.Join(
                    "&",
                    url.QueryParams
                        .OrderBy(
                            p => p.Name
                        )
                        .Select(
                            p => $"{p.Name}={Uri.EscapeDataString(p.Value.ToString() ?? "")}"
                        )
                )  
            );

        /* create the crypto class we use to generate a signature for the request */
        var keySrting = this.options.APISecret + "&" + (token?.Secret ?? "");
        /* generate the signature and add it to our parameters */
        var keyBytes = Encoding.UTF8.GetBytes(keySrting);
        HMACSHA1 hashAlgorithm = new HMACSHA1(keyBytes);

        return url.SetQueryParam(
            "oauth_signature",
            Convert.ToBase64String(
                hashAlgorithm.ComputeHash(
                    Encoding.UTF8.GetBytes(baseStr)
                )
            )
        );
    }


    public class Options
    {
        public string APIKey { get; set; } = "";
        public string APISecret { get; set; } = "";
    }
}

