using System.Management.Automation;
using FlickrNetCore;
using FlickrNetCore.Resources;

namespace PoshFlickr.Commands;

[Cmdlet(VerbsCommon.Get, "FlickrAlbumPhotos")]
[OutputType(typeof(AlbumPhotoResource))]
public class GetFlickrAlbumPhotos : StateRequiredAsyncPSCmdlet
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
        Position = 1,
        ValueFromPipeline = false,
        ValueFromPipelineByPropertyName = true
    )]
    public string OwnerId { get; set; } = null!;

    [Parameter(
        ValueFromPipeline = false,
        ValueFromPipelineByPropertyName = false
    )]
    public string[] Property { get; set; } = Array.Empty<string>();

    protected override async Task ProcessRecordAsync(
        SharedState state,
        CancellationToken cancellationToken
    )
    {

        foreach (var id in Id)
        {
            WriteObject(
                await state.Client.Albums.FetchPhotos(
                    OwnerId,
                    id,
                    state.AccessToken,
                    cancellationToken,
                    Property
                ),
                true
            );
        }
                

        
    }


}

