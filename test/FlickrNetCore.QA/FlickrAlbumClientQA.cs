namespace FlickrNetCore.QA;

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
            "72157638033710966"
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
            "72157638033710966"
        );

    }
    
}
