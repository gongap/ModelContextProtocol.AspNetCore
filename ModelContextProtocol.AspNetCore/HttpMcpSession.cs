﻿using ModelContextProtocol.AspNetCore.Stateless;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.Security.Claims;

namespace ModelContextProtocol.AspNetCore;

internal sealed class HttpMcpSession<TTransport>(
    string sessionId,
    TTransport transport,
    UserIdClaim? userId,
    TimeProvider timeProvider) : IAsyncDisposable
    where TTransport : ITransport
{
    private int _referenceCount;
    private int _getRequestStarted;
    private CancellationTokenSource _disposeCts = new();

    public string Id { get; } = sessionId;
    public TTransport Transport { get; } = transport;
    public UserIdClaim? UserIdClaim { get; } = userId;

    public CancellationToken SessionClosed => _disposeCts.Token;

    public bool IsActive => !SessionClosed.IsCancellationRequested && _referenceCount > 0;
    public long LastActivityTicks { get; private set; } = timeProvider.GetTimestamp();

    private TimeProvider TimeProvider => timeProvider;

    public IMcpServer? Server { get; set; }
    public Task? ServerRunTask { get; set; }

    public IDisposable AcquireReference()
    {
        Interlocked.Increment(ref _referenceCount);
        return new UnreferenceDisposable(this);
    }

    public bool TryStartGetRequest() => Interlocked.Exchange(ref _getRequestStarted, 1) == 0;

    public async ValueTask DisposeAsync()
    {
        try
        {
            _disposeCts.Cancel();

            if (ServerRunTask is not null)
            {
                await ServerRunTask;
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            try
            {
                if (Server is not null)
                {
                    await Server.DisposeAsync();
                }
            }
            finally
            {
                await Transport.DisposeAsync();
                _disposeCts.Dispose();
            }
        }
    }

    public bool HasSameUserId(ClaimsPrincipal user)
        => UserIdClaim == StreamableHttpHandler.GetUserIdClaim(user);

    private sealed class UnreferenceDisposable(HttpMcpSession<TTransport> session) : IDisposable
    {
        public void Dispose()
        {
            if (Interlocked.Decrement(ref session._referenceCount) == 0)
            {
                session.LastActivityTicks = session.TimeProvider.GetTimestamp();
            }
        }
    }
}
