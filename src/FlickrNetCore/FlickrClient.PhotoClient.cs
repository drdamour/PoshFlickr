using System;
using System.Net.Http;
using System.Net.Http.Json;
using FlickrNetCore.Resources;
using FlickrNetCore.Responses;
using Flurl;

namespace FlickrNetCore;

public partial class FlickrClient
{
    public readonly PhotoClient Photos;

    public class PhotoClient
    {
        private readonly FlickrClient fc;

        internal PhotoClient(
            FlickrClient fc
        )
        {
            this.fc = fc;
        }

        public async Task<PhotoResource> FetchInfo(
            string id,
            CancellationToken cancellationToken = default
        )
        {
            var result = await fc.httpClient.GetAsync(
                baseHref
                    .SetQueryParam(
                        "method",
                        "flickr.photos.getInfo"
                    )
                    .SetQueryParam(
                        "api_key",
                        fc.options.APIKey
                    )
                    .SetQueryParam(
                        "photo_id",
                        id
                    ),
                cancellationToken
            );

            var r = await result.Content.ReadFromJsonAsync<PhotoResponse>(
                cancellationToken: cancellationToken
            );

            return r.Photo;

        }
    }


}

