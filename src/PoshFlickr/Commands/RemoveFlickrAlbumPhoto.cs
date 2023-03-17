using System.Management.Automation;
using FlickrNetCore;
using FlickrNetCore.Resources;

namespace PoshFlickr.Commands;

[Cmdlet(VerbsCommon.Remove, "FlickrAlbumPhoto")]
[OutputType(typeof(AlbumPhotoResource))]
public class RemoveFlickrAlbumPhoto : StateRequiredAsyncPSCmdlet
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
        Position = 0,
        ValueFromPipeline = false,
        ValueFromPipelineByPropertyName = true,
        ParameterSetName = "byId"
    )]
    public string AlbumId { get; set; } = "";

    protected override async Task ProcessRecordAsync(
        SharedState state,
        CancellationToken cancellationToken
    )
    {

        foreach (var id in Id)
        {
            await state.Client.Albums.RemovePhoto(
                id,
                AlbumId,
                state.AccessToken,
                cancellationToken
            );

            WriteObject(
                $"photo [{id}] removed from album [{AlbumId}]"
            );
        }
                

        
    }


}

