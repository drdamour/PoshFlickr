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
        private readonly FlickrClient flickr;

        internal AlbumClient(
            FlickrClient fc
        )
        {
            this.flickr = fc;
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
            var result = await flickr.httpClient.GetAsync(
                flickr.MakeOAuthUrl(
                    flickr.StartFlickrUrl(
                            "flickr.photosets.getInfo"
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

        //https://www.flickr.com/services/api/flickr.photosets.getList.html

        //TODO: convert to IAsyncEnumerable that goes through the whole page collection wiht params
        //but not really needed since this supposedly returns all the items in a page...
        public async Task<IEnumerable<AlbumResource>> FetchList(
            AccessToken? token,
            string? ownerId = null,
            CancellationToken cancellationToken = default
        )
        {
            var url = flickr
                    .StartFlickrUrl(
                        "flickr.photosets.getList"
                    );

            if( ownerId != null)
            {
                url.SetQueryParam(
                    "user_id",
                    ownerId
                );
            }

            var result = await flickr.httpClient.GetAsync(
                flickr.MakeOAuthUrl(
                    url,
                    token
                ),
                cancellationToken
            );

            var r = await result.Content.ReadFromJsonAsync<PhotosetsPageResponse>(
                cancellationToken: cancellationToken
            );

            //TODO check stat and null

            return r.Page.Albums;

        }
    }


}

