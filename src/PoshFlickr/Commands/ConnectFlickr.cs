using System.Management.Automation;
using System.Management.Automation.Host;
using FlickrNetCore;
using FlickrNetCore.Resources;

namespace PoshFlickr.Commands;

[Cmdlet(VerbsCommunications.Connect, "Flickr")]
[OutputType(typeof(PhotoResource))]
public class ConnectFlickr : AsyncPSCmdlet
{
    //TODO: there should be a way to not pass a secret for anonymous/logged off browsing with a consumer key only
    //TODO: help should describe how to gen a consumer key, link you there

    [Parameter(
        Mandatory = true,
        Position = 0
    )]
    public string ConsumerKey { get; set; } = "";

    [Parameter(
        Mandatory = true,
        Position = 1
    )]
    public string ConsumerSecret { get; set; } = "";


    // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
    protected override async Task ProcessRecordAsync(
        CancellationToken cancellationToken
    )
    {

        //todo: leverage https://learn.microsoft.com/en-us/powershell/scripting/developer/cmdlet/strongly-encouraged-development-guidelines?view=powershell-7.3#use-the-host-interfaces

        var state = new SharedState(
            new FlickrClient(
                new HttpClient(),
                new FlickrClient.Options()
                {
                    APIKey = this.ConsumerKey,
                    APISecret = this.ConsumerSecret
                }
            )
        );

        this.SessionState.PSVariable.Set(
            SharedState.StateKey,
            state    
        );

        

        var reqToken = await state.Client.FetchRequestToken(cancellationToken);

        Dictionary<string, PSObject> answer = null!;
        do {
            answer = this.Host.UI.Prompt(
                "verification token needed",
                $" nav to {reqToken.MakeAuthorizeHref()} val",
                new System.Collections.ObjectModel.Collection<FieldDescription>()
                {
                    new FieldDescription("token")
                }
            );
        } while (!answer.ContainsKey("token") || string.IsNullOrWhiteSpace(answer["token"].ToString()));

        state = state with
        {
            AccessToken = await state.Client.FetchAccessToken(
                reqToken,
                answer["token"].ToString(),
                cancellationToken
            )
        };

        this.SessionState.PSVariable.Set(
            SharedState.StateKey,
            state
        );

        WriteObject(
            $"token aquired for {state.AccessToken.UserName}"
        );
    }

}
