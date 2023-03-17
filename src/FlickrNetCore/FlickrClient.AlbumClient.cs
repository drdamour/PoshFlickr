using System.Net.Http.Json;
using FlickrNetCore.Auth;
using FlickrNetCore.Resources;
using FlickrNetCore.Responses;

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
            string id,
            AccessToken token,
            CancellationToken cancellationToken = default
        )
        {
            return FetchInfo(
                token.UserId,
                id,
                token,
                cancellationToken
            );
        }

        public async Task<AlbumResource> FetchInfo(
            string ownerId,
            string id,
            AccessToken? token = null,
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
            string? ownerId = null,
            AccessToken? token = null,
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

            var r = await result.Content.ReadFromJsonAsync<AlbumsPageResponse>(
                cancellationToken: cancellationToken
            );

            //TODO check stat and null

            return r.Page.Albums;

        }


        //https://www.flickr.com/services/api/flickr.photosets.getPhotos.html

        public Task<IEnumerable<AlbumPhotoResource>> FetchPhotos(
            string id,
            AccessToken token,
            CancellationToken cancellationToken = default,
            params string[] extraProperties
        )
        {
            return FetchPhotos(
                token.UserId,
                id,
                token,
                cancellationToken,
                extraProperties
            );
        }

        //TODO: convert to IAsyncEnumerable that goes through the whole page collection wiht params
        //but not really needed since this supposedly returns all the items in a page...

        //TODO: consider renaming FetchMedia to encompass video vs photo
        //TODO: support privacy filter
        public async Task<IEnumerable<AlbumPhotoResource>> FetchPhotos(
            string ownerId,
            string id,
            AccessToken? token = null,
            CancellationToken cancellationToken = default,
            params string[] extraProperties
        )
        {

            var result = await flickr.httpClient.GetAsync(
                flickr.MakeOAuthUrl(
                    flickr
                        .StartFlickrUrl(
                            "flickr.photosets.getPhotos"
                        )
                        .SetQueryParam(
                            "user_id",
                            ownerId
                        )
                        .SetQueryParam(
                            "photoset_id",
                            id
                        )
                        .SetQueryParam(
                            "extras",
                            string.Join(",", extraProperties)
                        )
                        ,
                    token
                ),
                cancellationToken
            );

            var r = await result.Content.ReadFromJsonAsync<AlbumPhotosPageResponse>(
                cancellationToken: cancellationToken
            );

            //TODO check stat and null

            return r.Page.Photos.Select(
                p => p with
                {
                    AlbumId = r.Page.Id
                }
            );

        }


        //https://www.flickr.com/services/api/flickr.photosets.removePhoto.html

        public async Task RemovePhoto(
            string photoId,
            string albumId,
            AccessToken token,
            CancellationToken cancellationToken = default
        )
        {
            

            var result = await flickr.httpClient.PostAsync(
                flickr.MakeOAuthUrl(
                    flickr
                        .StartFlickrUrl(
                            "flickr.photosets.removePhoto"
                        )
                        .SetQueryParam(
                            "photo_id",
                            photoId
                        )
                        .SetQueryParam(
                            "photoset_id",
                            albumId
                        ),
                    token,
                    HttpMethod.Post
                ),
                new StringContent(""),
                cancellationToken
            );


            //TODO check reponse content for failure info

        }


    }


}

