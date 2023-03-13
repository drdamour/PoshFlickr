namespace FlickrNetCore.QA;

public class FlickrClientQA
{
    [Fact]
    public async void TestFetchPhotoInfoNoKey()
    {
        var subject = new FlickrClient(
            new HttpClient(),
            new FlickrClient.Options()
            {
                APIKey = "",
            }
        );

        var result = await subject.FetchInfo("52741293151");

    }


    [Fact]
    public async void TestFetchPhotoInfoWithKey()
    {
        var subject = new FlickrClient(
            new HttpClient(),
            new FlickrClient.Options()
            {
                APIKey = "cca4104b1560829a987156c7603de1dc",
            }
        );

        var result = await subject.FetchInfo("52741293151");

    }
    
}
