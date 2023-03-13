using System;
using System.Net.Http;
using System.Net.Http.Json;
using FlickrNetCore.Resources;
using Flurl;

namespace FlickrNetCore;

public class FlickrClient
{
    private const string baseHref = "https://api.flickr.com/services/rest?format=json&nojsoncallback=1";

    private readonly HttpClient httpClient;
    private readonly Options options;

    public FlickrClient(
        HttpClient httpClient,
        Options options
    )
    {
        this.httpClient = httpClient;
        this.options = options;
    }

    public async Task<PhotoResource> FetchInfo(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        var result = await httpClient.GetAsync(
            baseHref
                .SetQueryParam(
                    "method",
                    "flickr.photos.getInfo"
                )
                .SetQueryParam(
                    "method",
                    "flickr.photos.getInfo"
                )
                .SetQueryParam(
                    "api_key",
                    this.options.APIKey
                )
                .SetQueryParam(
                    "photo_id",
                    id
                ),
            cancellationToken
        );

        var r = await result.Content.ReadFromJsonAsync<PhotoResource>(
            cancellationToken: cancellationToken
        );

        return r;
        
    }

    public class Options
    {
        public string APIKey { get; set; } = "";
    }
}

