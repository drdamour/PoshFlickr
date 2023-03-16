using System.Management.Automation;
using PoshFlickr;

namespace FlickrNetCore;

//stole from https://github.com/felixfbecker/PSKubectl/blob/70356d1ece3bda420d4ca87d5b8f9b538d2668c5/src/ThreadAffinitiveSynchronizationContext.cs
//and https://github.com/felixfbecker/PSKubectl/blob/70356d1ece3bda420d4ca87d5b8f9b538d2668c5/src/AsyncCmdlet.cs
//because https://github.com/PowerShell/PowerShell/issues/7690


/// <summary>
///		Base class for Cmdlets that run asynchronously.
/// </summary>
/// <remarks>
///		Inherit from this class if your Cmdlet needs to use <c>async</c> / <c>await</c> functionality.
/// </remarks>
public abstract class StateRequiredAsyncPSCmdlet
    : AsyncPSCmdlet
{

    protected abstract Task ProcessRecordAsync(
        SharedState state,
        CancellationToken cancellationToken
    );


    override protected Task ProcessRecordAsync(
        CancellationToken cancellationToken
    )
    {
        if (this.SessionState.TryGetState(out var state))
        {
            return ProcessRecordAsync(
                state,
                cancellationToken
            );
        }


        WriteError(
            new ErrorRecord(
                new Exception("no connection established, use Connect-Flickr to establish a connection"),
                "123",
                ErrorCategory.ConnectionError,
                null
            )
        );

        return Task.CompletedTask;

    }


}