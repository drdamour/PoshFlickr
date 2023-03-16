using System.Collections.Concurrent;
using System.Management.Automation;
using System.Runtime.ExceptionServices;


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
public abstract class AsyncPSCmdlet
    : PSCmdlet, IDisposable
{
    /// <summary>
    ///		The source for cancellation tokens that can be used to cancel the operation.
    /// </summary>
    readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();


    /// <summary>
    ///		Initialise the <see cref="AsyncPSCmdlet"/>.
    /// </summary>
    protected AsyncPSCmdlet()
    {
    }


    /// <summary>
    ///		Finaliser for <see cref="AsyncPSCmdlet"/>.
    /// </summary>
    ~AsyncPSCmdlet()
    {
        Dispose(false);
    }


    /// <summary>
    ///		Dispose of resources being used by the Cmdlet.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    /// <summary>
    ///		Dispose of resources being used by the Cmdlet.
    /// </summary>
    /// <param name="disposing">
    ///		Explicit disposal?
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            _cancellationSource.Dispose();
    }



    /// <summary>
    ///		Asynchronously perform Cmdlet pre-processing.
    /// </summary>
    /// <param name="cancellationToken">
    ///		A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    ///		A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    protected virtual Task BeginProcessingAsync(
        CancellationToken cancellationToken
    )
    {
        return Task.CompletedTask;
    }




    /// <summary>
    ///		Asynchronously perform Cmdlet processing.
    /// </summary>
    /// <param name="cancellationToken">
    ///		A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    ///		A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    protected abstract Task ProcessRecordAsync(
        CancellationToken cancellationToken
    );


    /// <summary>
    ///		Asynchronously perform Cmdlet post-processing.
    /// </summary>
    /// <param name="cancellationToken">
    ///		A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    ///		A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    protected virtual Task EndProcessingAsync(
        CancellationToken cancellationToken
    )
    {
        return Task.CompletedTask;
    }



    /// <summary>
    ///		Perform Cmdlet pre-processing.
    /// </summary>
    protected sealed override void BeginProcessing()
    {
        ThreadAffinitiveSynchronizationContext.RunSynchronized(
            () => BeginProcessingAsync(_cancellationSource.Token)
        );
    }


    /// <summary>
    ///		Perform Cmdlet processing.
    /// </summary>
    protected sealed override void ProcessRecord()
    {
        ThreadAffinitiveSynchronizationContext.RunSynchronized(
            () => ProcessRecordAsync(_cancellationSource.Token)
        );
    }


    /// <summary>
    ///		Perform Cmdlet post-processing.
    /// </summary>
    protected sealed override void EndProcessing()
    {
        ThreadAffinitiveSynchronizationContext.RunSynchronized(
            () => EndProcessingAsync(_cancellationSource.Token)
        );
    }


    /// <summary>
    ///		Interrupt Cmdlet processing (if possible).
    /// </summary>
    protected sealed override void StopProcessing()
    {
        _cancellationSource.Cancel();


        base.StopProcessing();
    }

}


/// <summary>
///		A synchronisation context that runs all calls scheduled on it (via <see cref="SynchronizationContext.Post"/>) on a single thread.
/// </summary>
/// <remarks>
///		With thanks to Stephen Toub.
/// </remarks>
public sealed class ThreadAffinitiveSynchronizationContext
    : SynchronizationContext, IDisposable
{
    /// <summary>
    ///		A blocking collection (effectively a queue) of work items to execute, consisting of callback delegates and their callback state (if any).
    /// </summary>
    BlockingCollection<KeyValuePair<SendOrPostCallback, object?>> _workItemQueue = new BlockingCollection<KeyValuePair<SendOrPostCallback, object?>>();


    /// <summary>
    ///		Create a new thread-affinitive synchronisation context.
    /// </summary>
    ThreadAffinitiveSynchronizationContext()
    {
    }


    /// <summary>
    ///		Dispose of resources being used by the synchronisation context.
    /// </summary>
    void IDisposable.Dispose()
    {
        if (_workItemQueue != null)
        {
            _workItemQueue.Dispose();
            _workItemQueue = null!;
        }
    }


    /// <summary>
    ///		Check if the synchronisation context has been disposed.
    /// </summary>
    void CheckDisposed()
    {
        if (_workItemQueue == null)
            throw new ObjectDisposedException(GetType().Name);
    }


    /// <summary>
    ///		Run the message pump for the callback queue on the current thread.
    /// </summary>
    void RunMessagePump()
    {
        CheckDisposed();


        KeyValuePair<SendOrPostCallback, object?> workItem;
        while (_workItemQueue.TryTake(out workItem, Timeout.InfiniteTimeSpan))
        {
            workItem.Key(workItem.Value);


            // Has the synchronisation context been disposed?
            if (_workItemQueue == null)
                break;
        }
    }


    /// <summary>
    ///		Terminate the message pump once all callbacks have completed.
    /// </summary>
    void TerminateMessagePump()
    {
        CheckDisposed();


        _workItemQueue.CompleteAdding();
    }


    /// <summary>
    ///		Dispatch an asynchronous message to the synchronization context.
    /// </summary>
    /// <param name="callback">
    ///		The <see cref="SendOrPostCallback"/> delegate to call in the synchronisation context.
    /// </param>
    /// <param name="callbackState">
    ///		Optional state data passed to the callback.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///		The message pump has already been started, and then terminated by calling <see cref="TerminateMessagePump"/>.
    /// </exception>
    public override void Post(
        SendOrPostCallback callback,
        object? callbackState
    )
    {
        if (callback == null)
        {
            throw new ArgumentNullException(nameof(callback));
        }


        CheckDisposed();


        try
        {
            _workItemQueue.Add(
                new KeyValuePair<SendOrPostCallback, object?>(
                    key: callback,
                    value: callbackState
                )
            );
        }
        catch (InvalidOperationException eMessagePumpAlreadyTerminated)
        {
            throw new InvalidOperationException(
                "Cannot enqueue the specified callback because the synchronisation context's message pump has already been terminated.",
                eMessagePumpAlreadyTerminated
                );
        }
    }


    /// <summary>
    ///		Run an asynchronous operation using the current thread as its synchronisation context.
    /// </summary>
    /// <param name="asyncOperation">
    ///		A <see cref="Func{TResult}"/> delegate representing the asynchronous operation to run.
    /// </param>
    public static void RunSynchronized(Func<Task> asyncOperation)
    {
        if (asyncOperation == null)
            throw new ArgumentNullException(nameof(asyncOperation));


        var savedContext = Current;
        try
        {
            using (ThreadAffinitiveSynchronizationContext synchronizationContext = new ThreadAffinitiveSynchronizationContext())
            {
                SetSynchronizationContext(synchronizationContext);


                Task rootOperationTask = asyncOperation();
                if (rootOperationTask == null)
                    throw new InvalidOperationException("The asynchronous operation delegate cannot return null.");


                rootOperationTask.ContinueWith(
                    operationTask =>
                        synchronizationContext.TerminateMessagePump(),
                    scheduler:
                        TaskScheduler.Default
                );


                synchronizationContext.RunMessagePump();


                try
                {
                    rootOperationTask
                        .GetAwaiter()
                        .GetResult();
                }
                catch (AggregateException eWaitForTask) // The TPL will almost always wrap an AggregateException around any exception thrown by the async operation.
                {
                    // Is this just a wrapped exception?
                    AggregateException flattenedAggregate = eWaitForTask.Flatten();
                    if (flattenedAggregate.InnerExceptions.Count != 1)
                        throw; // Nope, genuine aggregate.


                    // Yep, so rethrow (preserving original stack-trace).
                    ExceptionDispatchInfo
                        .Capture(
                            flattenedAggregate
                                .InnerExceptions[0]
                        )
                        .Throw();
                }
            }
        }
        finally
        {
            SetSynchronizationContext(savedContext);
        }
    }


    /// <summary>
    ///		Run an asynchronous operation using the current thread as its synchronisation context.
    /// </summary>
    /// <typeparam name="TResult">
    ///		The operation result type.
    /// </typeparam>
    /// <param name="asyncOperation">
    ///		A <see cref="Func{TResult}"/> delegate representing the asynchronous operation to run.
    /// </param>
    /// <returns>
    ///		The operation result.
    /// </returns>
    public static TResult RunSynchronized<TResult>(Func<Task<TResult>> asyncOperation)
    {
        if (asyncOperation == null)
            throw new ArgumentNullException(nameof(asyncOperation));


        var savedContext = Current;
        try
        {
            using (ThreadAffinitiveSynchronizationContext synchronizationContext = new ThreadAffinitiveSynchronizationContext())
            {
                SetSynchronizationContext(synchronizationContext);


                Task<TResult> rootOperationTask = asyncOperation();
                if (rootOperationTask == null)
                    throw new InvalidOperationException("The asynchronous operation delegate cannot return null.");


                rootOperationTask.ContinueWith(
                    operationTask =>
                        synchronizationContext.TerminateMessagePump(),
                    scheduler:
                        TaskScheduler.Default
                );


                synchronizationContext.RunMessagePump();


                try
                {
                    return
                        rootOperationTask
                            .GetAwaiter()
                            .GetResult();
                }
                catch (AggregateException eWaitForTask) // The TPL will almost always wrap an AggregateException around any exception thrown by the async operation.
                {
                    // Is this just a wrapped exception?
                    AggregateException flattenedAggregate = eWaitForTask.Flatten();
                    if (flattenedAggregate.InnerExceptions.Count != 1)
                        throw; // Nope, genuine aggregate.


                    // Yep, so rethrow (preserving original stack-trace).
                    ExceptionDispatchInfo
                        .Capture(
                            flattenedAggregate
                                .InnerExceptions[0]
                        )
                        .Throw();


                    throw; // Never reached.
                }
            }
        }
        finally
        {
            SetSynchronizationContext(savedContext);
        }
    }
}