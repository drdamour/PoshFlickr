using Flurl;

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
    public async void TestFetchRequestTokenWithKeyNoSecret()
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
    public async void TestFetchRequestTokenWithKeyAndSecret()
    {

        var subject = new FlickrClient(
            new HttpClient(),
            new FlickrClient.Options()
            {
                APIKey = "cca4104b1560829a987156c7603de1dc",
                APISecret = Secrets.VerifyAPISecretSet(),
            }
        );

        var requestToken = await subject.FetchRequestToken();

        var authorizeHref = requestToken.MakeAuthorizeHref(Auth.AuthLevel.Write);

        
        //TODO: some asserts

    }


    [Fact]
    public async void TestFetchAccessTokenWithKeyAndSecret()
    {

        var subject = new FlickrClient(
            new HttpClient(),
            new FlickrClient.Options()
            {
                APIKey = "cca4104b1560829a987156c7603de1dc",
                APISecret = Secrets.VerifyAPISecretSet(),
            }
        );

        var requestToken = await subject.FetchRequestToken();

        var authorizeHref = requestToken.MakeAuthorizeHref(Auth.AuthLevel.Write);

        //TODO: open a browser somehow

        //TODO: capture a verify token somehow, maybe a localhost callback?

        var verifyToken = "1234";

        var acessToken = await subject.FetchAccessToken(
            requestToken,
            verifyToken
        );


        //TODO: some asserts

    }





}
