﻿using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PoshFlickr.Commands;

[Cmdlet(VerbsCommunications.Connect,"Flickr")]
[OutputType(typeof(FavoriteStuff))]
public class ConnectFlickr : PSCmdlet
{
    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    public int FavoriteNumber { get; set; }

    [Parameter(
        Position = 1,
        ValueFromPipelineByPropertyName = true)]
    [ValidateSet("Cat", "Dog", "Horse")]
    public string FavoritePet { get; set; } = "Dog";

    // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
    protected override void BeginProcessing()
    {
        WriteVerbose("Begin!");
    }

    // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
    protected override void ProcessRecord()
    {
        //todo: leverage https://learn.microsoft.com/en-us/powershell/scripting/developer/cmdlet/strongly-encouraged-development-guidelines?view=powershell-7.3#use-the-host-interfaces
        WriteObject(new FavoriteStuff { 
            FavoriteNumber = FavoriteNumber,
            FavoritePet = FavoritePet
        });
    }

    // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
    protected override void EndProcessing()
    {
        WriteVerbose("End!");
    }
}

public class FavoriteStuff
{
    public int FavoriteNumber { get; set; }
    public string FavoritePet { get; set; }
}
