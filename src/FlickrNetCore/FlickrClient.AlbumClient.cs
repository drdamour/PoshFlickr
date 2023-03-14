using System;
using System.Net.Http;
using System.Net.Http.Json;
using FlickrNetCore.Resources;
using Flurl;

namespace FlickrNetCore;

public partial class FlickrClient
{

    public readonly AlbumClient Albums;

    public class AlbumClient
    {
        private readonly FlickrClient fc;

        internal AlbumClient(
            FlickrClient fc
        )
        {
            this.fc = fc;
        }

        //https://www.flickr.com/services/api/flickr.photosets.getInfo.html

        public async Task<AlbumResource> FetchInfo(
            string ownerId,
            string id,
            CancellationToken cancellationToken = default
        )
        {
            var result = await fc.httpClient.GetAsync(
                baseHref
                    .SetQueryParam(
                        "method",
                        "flickr.photosets.getInfo"
                    )
                    .SetQueryParam(
                        "api_key",
                        fc.options.APIKey
                    )
                    .SetQueryParam(
                        "user_id",
                        ownerId
                    )
                    .SetQueryParam(
                        "photoset_id",
                        id
                    ),
                cancellationToken
            );

            var r = await result.Content.ReadFromJsonAsync<AlbumResource>(
                cancellationToken: cancellationToken
            );

            return r;

        }
    }


}

