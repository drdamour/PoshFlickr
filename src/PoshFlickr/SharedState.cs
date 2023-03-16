using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using FlickrNetCore;
using FlickrNetCore.Auth;

namespace PoshFlickr;

//HACK: this cant be right...there must be some pace to store
public record SharedState(
    FlickrClient Client,
    AccessToken? AccessToken = null
)
{
    public const string StateKey = "PoshFlickr.State";

    public bool HasToken(
        [NotNullWhen(true)] out AccessToken? token
    )
    {
        token = this.AccessToken;
        return AccessToken != null;
    }
}

public static class StateUtils
{
    public static bool TryGetState(
        this SessionState session,
        [NotNullWhen(true)] out SharedState? state
    )
    {
        state = session.PSVariable.Get(
            SharedState.StateKey
        )?.Value as SharedState;

        return state != null;
    }
}

