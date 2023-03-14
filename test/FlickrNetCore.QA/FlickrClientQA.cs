namespace FlickrNetCore.QA;

public class FlickrClientQA
{
    [Fact]
    public async void TestFetchRequestTokenWithNoKeyNoSecret()
    {
        var subject = new FlickrClient(
            new HttpClient(),
            new FlickrClient.Options()
            {
                APIKey = "",
            }
        );

        //TODO: should throw consumer key unknown
        await subject.FetchRequestToken();

    }


    [Fact]
    public async void TestFetchInfoWithKeyNoSecret()
    {
        var subject = new FlickrClient(
            new HttpClient(),
            new FlickrClient.Options()
            {
                APIKey = "cca4104b1560829a987156c7603de1dc",
            }
        );

        //TODO: should throw signature invalid, or maybe api key invalid casue it's blank?
        await subject.FetchRequestToken();

    }


    [Fact]
    public async void TestFetchInfoWithKeyAndSecret()
    {
        var subject = new FlickrClient(
            new HttpClient(),
            new FlickrClient.Options()
            {
                APIKey = "cca4104b1560829a987156c7603de1dc",
                APISecret = "[REDACTED]",
            }
        );

        //TODO: should throw signature invalid, or maybe api key invalid casue it's blank?
        await subject.FetchRequestToken();

    }

}
