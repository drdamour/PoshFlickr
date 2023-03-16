using System.Management.Automation;
using FlickrNetCore;
using FlickrNetCore.Resources;

namespace PoshFlickr.Commands;

[Cmdlet(VerbsCommon.Get, "FlickrAlbum")]
[OutputType(typeof(PhotoResource))]
public class GetFlickrAlbum : StateRequiredAsyncPSCmdlet
{
    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true
    )]
    public string[] Id { get; set; } = Array.Empty<string>();


    [Parameter(
        Mandatory = false,
        Position = 1,
        ValueFromPipeline = false,
        ValueFromPipelineByPropertyName = true
    )]
    public string UserId { get; set; } = "";

    protected override async Task ProcessRecordAsync(
        SharedState state,
        CancellationToken cancellationToken
    )
    {


        if (string.IsNullOrWhiteSpace(UserId))
        {
            if (state.HasToken(out var token))
            {
                foreach (var id in Id)
                {
                    WriteObject(
                        await state.Client.Albums.FetchInfo(
                            token,
                            id
                        )
                    );
                }
            }
            else
            {
                throw new Exception("no UserId supplied and no access token present. Either supply UserId or use Connect-Flickr to establish an access token");
            }

        }
        else
        {
            foreach (var id in Id)
            {
                WriteObject(
                    await state.Client.Albums.FetchInfo(
                        state.AccessToken,
                        UserId,
                        id
                    )
                );
            }
        }


       
        

        
    }


}

