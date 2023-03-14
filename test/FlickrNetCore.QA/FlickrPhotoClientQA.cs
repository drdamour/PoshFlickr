namespace FlickrNetCore.QA;

public class FlickrPhotoClientQA
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

        var result = await subject.Photos.FetchInfo("52741293151");

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

        var result = await subject.Photos.FetchInfo("52741293151");

    }
    
}
