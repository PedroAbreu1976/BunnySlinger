using RabbitMQ.Client;

namespace BunnySlinger.Options;

/// <summary>
/// Represents configuration options for establishing and managing RabbitMQ connections.
/// </summary>
/// <remarks>This class provides a set of properties to configure various aspects of RabbitMQ connections,  such
/// as connection timeouts, authentication credentials, TLS settings, and protocol-specific options.  It acts as a
/// wrapper around the RabbitMQ <see cref="ConnectionFactory"/> to simplify configuration  and ensure consistency across
/// connections.</remarks>
public class BunnyMqConfiguration {
    private readonly ConnectionFactory _factory = new ConnectionFactory();

    /// <summary>
    /// Represents configuration options for a BunnyMQ connection.
    /// </summary>
    /// <remarks>This class provides options for configuring the BunnyMQ connection, including SSL
    /// settings.</remarks>
    public BunnyMqConfiguration() {
	    Ssl = new RabbitMqSslOption {
            ServerName = _factory.Ssl.ServerName,
            AcceptablePolicyErrors = _factory.Ssl.AcceptablePolicyErrors,
            CertPath = _factory.Ssl.CertPath,
            Enabled = _factory.Ssl.Enabled,
            Version = _factory.Ssl.Version
        };
    }

    /// <summary>
    /// Retrieves the configured connection factory instance.
    /// </summary>
    /// <remarks>The returned connection factory is pre-configured with the current SSL settings, including 
    /// server name, acceptable policy errors, certificate path, SSL enablement, and version.</remarks>
    /// <returns>An <see cref="IConnectionFactory"/> instance configured with the current SSL settings.</returns>
    public IConnectionFactory GetFactory() {
        _factory.Ssl.ServerName = Ssl.ServerName;
        _factory.Ssl.AcceptablePolicyErrors = Ssl.AcceptablePolicyErrors;
        _factory.Ssl.CertPath = Ssl.CertPath;
        _factory.Ssl.Enabled = Ssl.Enabled;
        _factory.Ssl.Version = Ssl.Version;
        return _factory;
    }


    /// <summary>The host to connect to.</summary>
    public string HostName {
	    get => _factory.HostName;
	    set => _factory.HostName = value;
    }

    /// <summary>
    /// Amount of time client will wait for before re-trying  to recover connection.
    /// </summary>
    public TimeSpan NetworkRecoveryInterval
    {
	    get => _factory.NetworkRecoveryInterval;
	    set => _factory.NetworkRecoveryInterval = value;
    }

    /// <summary>
    /// Amount of time protocol handshake operations are allowed to take before timing out.
    /// </summary>
    public TimeSpan HandshakeContinuationTimeout
    {
	    get => _factory.HandshakeContinuationTimeout;
	    set => _factory.HandshakeContinuationTimeout = value;
    }

    /// <summary>
    /// Amount of time protocol operations (e.g. <code>queue.declare</code>) are allowed to take before
    /// timing out.
    /// </summary>
    public TimeSpan ContinuationTimeout
    {
	    get => _factory.ContinuationTimeout;
	    set => _factory.ContinuationTimeout = value;
    }

    /// <summary>
    /// The port to connect on. <see cref="AmqpTcpEndpoint.UseDefaultPort"/> indicates the default for the protocol should be used.
    /// </summary>
    public int Port
    {
	    get => _factory.Port;
	    set => _factory.Port = value;
    }

    /// <summary>
    /// Timeout setting for connection attempts.
    /// </summary>
    public TimeSpan RequestedConnectionTimeout
    {
	    get => _factory.RequestedConnectionTimeout;
	    set => _factory.RequestedConnectionTimeout = value;
    }

    /// <summary>
    /// Timeout setting for socket read operations.
    /// </summary>
    public TimeSpan SocketReadTimeout
    {
	    get => _factory.SocketReadTimeout;
	    set => _factory.SocketReadTimeout = value;
    }

    /// <summary>
    /// Timeout setting for socket write operations.
    /// </summary>
    public TimeSpan SocketWriteTimeout
    {
	    get => _factory.SocketWriteTimeout;
	    set => _factory.SocketWriteTimeout = value;
    }

    
    /// <summary>
    /// TLS options setting.
    /// </summary>
    public RabbitMqSslOption Ssl { get; set; }

    /// <summary>
    /// Username to use when authenticating to the server.
    /// </summary>
    public string UserName
    {
	    get => _factory.UserName;
	    set => _factory.UserName = value;
    }

    /// <summary>
    /// Password to use when authenticating to the server.
    /// </summary>
    public string Password
    {
	    get => _factory.Password;
	    set => _factory.Password = value;
    }

    /// <summary>
    /// Maximum channel number to ask for.
    /// </summary>
    public ushort RequestedChannelMax
    {
	    get => _factory.RequestedChannelMax;
	    set => _factory.RequestedChannelMax = value;
    }

    /// <summary>
    /// Frame-max parameter to ask for (in bytes).
    /// </summary>
    public uint RequestedFrameMax
    {
	    get => _factory.RequestedFrameMax;
	    set => _factory.RequestedFrameMax = value;
    }

    /// <summary>
    /// Heartbeat timeout to use when negotiating with the server.
    /// </summary>
    public TimeSpan RequestedHeartbeat
    {
	    get => _factory.RequestedHeartbeat;
	    set => _factory.RequestedHeartbeat = value;
    }

    /// <summary>
    /// Virtual host to access during this connection.
    /// </summary>
    public string VirtualHost
    {
	    get => _factory.VirtualHost;
	    set => _factory.VirtualHost = value;
    }

    /// <summary>
    /// Maximum allowed message size, in bytes, from RabbitMQ.
    /// Corresponds to the <code>ConnectionFactory.DefaultMaxMessageSize</code> setting.
    /// </summary>
    public uint MaxInboundMessageBodySize
    {
	    get => _factory.MaxInboundMessageBodySize;
	    set => _factory.MaxInboundMessageBodySize = value;
    }
}
