using System.Management.Automation;
using FlickrNetCore;
using FlickrNetCore.Resources;

namespace PoshFlickr.Commands;

[Cmdlet(VerbsCommon.Get, "FlickrAlbum")]
[OutputType(typeof(AlbumPhotoResource))]
public class GetFlickrAlbum : StateRequiredAsyncPSCmdlet
{
    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        ParameterSetName = "byId"
    )]
    public string[] Id { get; set; } = Array.Empty<string>();

    [Parameter(
        Mandatory = true,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        ParameterSetName = "byName"
    )]
    public string[] Name { get; set; } = Array.Empty<string>();


    [Parameter(
        Mandatory = false,
        Position = 1,
        ValueFromPipeline = false,
        ValueFromPipelineByPropertyName = true
    )]
    public string? OwnerId { get; set; } = null;

    protected override async Task ProcessRecordAsync(
        SharedState state,
        CancellationToken cancellationToken
    )
    {
        if (Id.Length > 0)
        {
            if (string.IsNullOrWhiteSpace(OwnerId))
            {
                if (state.HasToken(out var token))
                {
                    foreach (var id in Id)
                    {
                        WriteObject(
                            await state.Client.Albums.FetchInfo(
                                id,
                                token,
                                cancellationToken
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
                            OwnerId,
                            id,
                            state.AccessToken,
                            cancellationToken
                        )
                    );
                }
            }
        }
        else
        //by name
        {
            var albums = await state.Client.Albums.FetchList(
                OwnerId,
                state.AccessToken,
                cancellationToken
            );

            foreach(var a in albums)
            {
                if(Name.Contains(a.Title))
                {
                    WriteObject(
                        a
                    );
                }
            }
        }

        
    }


}

