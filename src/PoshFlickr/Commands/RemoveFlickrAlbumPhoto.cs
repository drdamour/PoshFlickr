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
        //TODO: instead of this are you supposed to collect em all in here
        //then in endprocessing make a call to RemovePhotos so you turn single into bulk?
        //that makes some logical sense...

        //TODO: this definitely is supposed to opt into the should confirmation thingy nad have a --force
        //should level could be based on count if we do above thing

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

