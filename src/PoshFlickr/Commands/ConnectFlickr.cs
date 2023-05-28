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



        //todo: find an open port via https://stackoverflow.com/a/46666370/442773
        var openPort = 12000;
        //note redirect to http localhost was broken for a while but got fixed see https://www.flickr.com/groups/51035612836@N01/discuss/72157712651258108/
        var listenerHref = $"http://localhost:{openPort}/";
        var listener = new HttpListener();
        listener.Prefixes.Add(listenerHref);
        listener.Start();
        

        var reqToken = await state.Client.FetchRequestToken(
            //TOOD: support a -use-code prompt option to leveage oob/device code flow
            //"oob",
            listenerHref,
            cancellationToken
        );


        Process.Start(
            new ProcessStartInfo(
                reqToken
                    .MakeAuthorizeHref(
                        Level
                    )
            )
            {
                UseShellExecute = true
            }
        );


        while (listener.IsListening)
        {
            WriteVerbose("listener loop begin");
            //TODO: add some timeout
            var context = await listener.GetContextAsync()

            //var oauthtoken = context.Request.QueryString["oauth_token"];

            //todo: check this value as being there and not empty
            //if it is then give an html form to retry or exit
            var verifier = context.Request.QueryString["oauth_verifier"];


            state = state with
            {
                AccessToken = await state.Client.FetchAccessToken(
                    reqToken,
                    verifier,
                    cancellationToken
                ),
                PermissionLevel = Level,
            };

            var response = context.Response;
            response.StatusCode = 200;

            await response.OutputStream.WriteAsync(
                Encoding.UTF8.GetBytes(
                    "access token received, you can close this browser"
                )
            );

            response.Close();

            break;

        }

        listener.Stop();
        listener.Close();
        

        //var authHref = reqToken.MakeAuthorizeHref(Level);
        /*
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

        */
        

        this.SessionState.PSVariable.Set(
            SharedState.StateKey,
            state
        );

        WriteObject(
            $"token aquired for {state.AccessToken.UserName}"
        );
    }

}
