﻿namespace FlickrNetCore.QA;

public class FlickrAlbumClientQA
{
    [Fact]
    public async void TestFetchInfoNoKey()
    {
        var subject = new FlickrClient(
            new HttpClient(),
            new FlickrClient.Options()
            {
                APIKey = "",
            }
        );

        var result = await subject.Albums.FetchInfo(
            "63505810@N00",
            "72157638033710966",
            null!
        );

    }


    [Fact]
    public async void TestFetchInfoWithKey()
    {
        var subject = new FlickrClient(
            new HttpClient(),
            new FlickrClient.Options()
            {
                APIKey = "cca4104b1560829a987156c7603de1dc",
            }
        );

        var result = await subject.Albums.FetchInfo(
            "63505810@N00",
            "72157638033710966",
            null!
        );

        //TODO: assert failure
    }

    [Fact]
    public async void TestFetchInfoWithKeyAndSecret()
    {
        var subject = new FlickrClient(
            new HttpClient(),
            new FlickrClient.Options()
            {
                APIKey = "cca4104b1560829a987156c7603de1dc",
                APISecret = Secrets.VerifyAPISecretSet()
            }
        );

        var result = await subject.Albums.FetchInfo(
            "63505810@N00",
            "72157638033710966",
            Secrets.VerifyAccessTokenSet()
        );

        //TODO: assert failure
    }


    [Fact]
    public async void TestFetchListWithKeyAndSecret()
    {
        var subject = new FlickrClient(
            new HttpClient(),
            new FlickrClient.Options()
            {
                APIKey = "cca4104b1560829a987156c7603de1dc",
                APISecret = Secrets.VerifyAPISecretSet()
            }
        );

        var result = await subject.Albums.FetchList(
            "",
            Secrets.VerifyAccessTokenSet()
        );

        //TODO: assert failure
    }


    [Fact]
    public async void TestFetchPhotosWithKeyAndSecretNoExtras()
    {
        var subject = new FlickrClient(
            new HttpClient(),
            new FlickrClient.Options()
            {
                APIKey = "cca4104b1560829a987156c7603de1dc",
                APISecret = Secrets.VerifyAPISecretSet()
            }
        );

        var result = await subject.Albums.FetchPhotos(
            "72157638033710966",
            Secrets.VerifyAccessTokenSet()
        );

        //TODO: assert failure
    }

    [Fact]
    public async void TestFetchPhotosWithKeyAndSecretExtraAsTags()
    {
        var subject = new FlickrClient(
            new HttpClient(),
            new FlickrClient.Options()
            {
                APIKey = "cca4104b1560829a987156c7603de1dc",
                APISecret = Secrets.VerifyAPISecretSet()
            }
        );

        var result = await subject.Albums.FetchPhotos(
            "72157638033710966",
            Secrets.VerifyAccessTokenSet(),
            default,
            "tags"
        );

        //TODO: assert failure
    }

    [Fact]
    public async void TestRemovePhotoWithKeyAndSecret()
    {
        var subject = new FlickrClient(
            new HttpClient(),
            new FlickrClient.Options()
            {
                APIKey = "cca4104b1560829a987156c7603de1dc",
                APISecret = Secrets.VerifyAPISecretSet()
            }
        );

        await subject.Albums.RemovePhoto(
            "52741548084",
            "72157638033710966",
            Secrets.VerifyAccessTokenSet(),
            default
        );

        //TODO: assert failure
    }

}
