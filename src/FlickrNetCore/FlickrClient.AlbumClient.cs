using System;
using System.Net.Http;
using System.Net.Http.Json;
using FlickrNetCore.Auth;
using FlickrNetCore.Resources;
using FlickrNetCore.Responses;
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

        public Task<AlbumResource> FetchInfo(
            AccessToken token,
            string id,
            CancellationToken cancellationToken = default
        )
        {
            return FetchInfo(
                token,
                token.UserId,
                id,
                cancellationToken
            );
        }

        public async Task<AlbumResource> FetchInfo(
            AccessToken? token,
            string ownerId,
            string id,
            CancellationToken cancellationToken = default
        )
        {
            var result = await fc.httpClient.GetAsync(
                fc.MakeOAuthUrl(
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
                    token
                ),
                cancellationToken
            );

            var r = await result.Content.ReadFromJsonAsync<AlbumResponse>(
                cancellationToken: cancellationToken
            );

            //TODO check stat and null

            return r.Album;

        }
    }


}

