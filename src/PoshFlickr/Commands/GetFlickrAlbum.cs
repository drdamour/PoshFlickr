using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using FlickrNetCore;
using FlickrNetCore.Resources;

namespace PoshFlickr.Commands;

[Cmdlet(VerbsCommon.Get, "FlickrAlbum")]
[OutputType(typeof(PhotoResource))]
public class GetFlickrAlbum : AsyncPSCmdlet
{
    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true
    )]
    public string[] Id { get; set; } = Array.Empty<string>();


    protected override async Task BeginProcessingAsync(
        CancellationToken cancellationToken
    )
    {
        WriteVerbose("Begin!");
    }

    protected override async Task ProcessRecordAsync(
        CancellationToken cancellationToken
    )
    {
        this.
        WriteObject(
            $"ids [{Id.Length}]"
        );
    }

    protected override async Task EndProcessingAsync(
        CancellationToken cancellationToken
    )
    {
        WriteVerbose("End!");
    }
}

