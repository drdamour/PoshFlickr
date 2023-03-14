using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using FlickrNetCore.Resources;
using Flurl;

namespace FlickrNetCore;

public partial class FlickrClient
{
    private const string baseHref = "https://api.flickr.com/services/rest?format=json&nojsoncallback=1";
    private const string baseRequestTokenHref = "https://www.flickr.com/services/oauth/request_token";

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

    public async Task FetchRequestToken(
        string callbackHref = "",
        CancellationToken cancellationToken = default
    )
    {

        var result = await httpClient.GetAsync(
            MakeOAuthUrl(
                new Flurl.Url(baseRequestTokenHref)
            ),
            cancellationToken
        );

        var i = 0;
    }

    


    internal Flurl.Url MakeOAuthUrl(
        Flurl.Url url,
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
                67530
            //Random.Shared.Next(100, 99999)
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
                1678778363 //DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            )
            .SetQueryParam(
                "oauth_callback",
                "oob"
            );


            //parametersList.Add("oauth_token=" + oauthToken);
            //parametersList.Add("oauth_verifier=" + oauthVerifier);

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
        var keySrting = this.options.APISecret + "&";// + (oauthTokenSecret ?? "");
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

