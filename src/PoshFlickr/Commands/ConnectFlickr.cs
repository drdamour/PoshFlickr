using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text;
using FlickrNetCore;
using FlickrNetCore.Auth;
using FlickrNetCore.Resources;

namespace PoshFlickr.Commands;

[Cmdlet(VerbsCommunications.Connect, "Flickr")]
[OutputType(typeof(AlbumPhotoResource))]
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

    [Parameter()]
    public AuthLevel Level { get; set; } = AuthLevel.Write;

    //todo: support permissions flag


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

        //TOOD: support a -use-code prompt to leveage oob redirect with prompt as option
        //but by default use HttpListener with redirect to localhost
        //blocked by https://www.flickr.com/help/forum/en-us/72157712826332472/
        //or https://www.flickr.com/groups/51035612836@N01/discuss/72157712651258108/

        var reqToken = await state.Client.FetchRequestToken(
            "oob",
            cancellationToken
        );

        /*
        //todo: find an open port via https://stackoverflow.com/a/46666370/442773
        var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:12000/");
        listener.Start();

        Process.Start(new ProcessStartInfo(reqToken.MakeAuthorizeHref()) { UseShellExecute = true });


        while (listener.IsListening)
        {
            WriteVerbose("listener loop");
            var context = await listener.GetContextAsync();


            var url = context.Request.Url.ToString();

            var response = context.Response;
            response.StatusCode = 200;
            response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes($"all good {url}"));
            response.Close();

            break;

        }

        listener.Stop();
        listener.Close();
        */

        var authHref = reqToken.MakeAuthorizeHref(Level);

        Dictionary<string, PSObject> answer = null!;
        do {
            Process.Start(new ProcessStartInfo(authHref) { UseShellExecute = true });

            answer = this.Host.UI.Prompt(
                "verification token needed",
                $" nav to {authHref} val",
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
            ),
            PermissionLevel = Level,
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
