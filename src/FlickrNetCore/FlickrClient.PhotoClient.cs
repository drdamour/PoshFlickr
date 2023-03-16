using System.Net.Http.Json;
using FlickrNetCore.Resources;
using FlickrNetCore.Responses;

namespace FlickrNetCore;

public partial class FlickrClient
{
    public readonly PhotoClient Photos;

    public class PhotoClient
    {
        private readonly FlickrClient flickr;

        internal PhotoClient(
            FlickrClient fc
        )
        {
            this.flickr = fc;
        }

        public async Task<PhotoResource> FetchInfo(
            string id,
            CancellationToken cancellationToken = default
        )
        {
            var result = await flickr.httpClient.GetAsync(
                flickr
                    .StartFlickrUrl(
                        "flickr.photos.getInfo"
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

