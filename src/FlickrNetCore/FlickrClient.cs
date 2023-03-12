using System;
using System.Net.Http;
using System.Net.Http.Json;
using FlickrNetCore.Resources;

namespace FlickrNetCore;

public class FlickrClient
{
    private readonly HttpClient httpClient;

    public FlickrClient(
        HttpClient httpClient,
        Options options
    )
    {
        this.httpClient = httpClient;
    }

    public async Task<PhotoResource> FetchInfo(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        await httpClient.GetFromJsonAsync<PhotoResource>()
        
    }

    public class Options
    {
        public string APIKey { get; set; } = "";
    }
}

