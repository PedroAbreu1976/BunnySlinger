using System.Net.Security;
using System.Security.Authentication;

namespace BunnySlinger.Options;

/// <summary>
/// Represents a set of configurable TLS options for a connection. Use this class to configure
/// TLS version used, client certificate list or file location, peer certificate verification
/// (validation) functions, expected server name (Subject Alternative Name or Common Name),
/// and so on.
/// </summary>
public class RabbitMqSslOption
{
	/// <summary>
	/// Reconfigures the instance to enable/use TLSv1.2.
	/// Only used in environments where System.Security.Authentication.SslProtocols.None
	/// is unavailable or effectively disabled, as reported by System.Net.ServicePointManager.
	/// </summary>
    public SslProtocols Version { get; set; } = SslProtocols.None;

	/// <summary>
	/// Retrieve or set the set of TLS policy (peer verification) errors that are deemed acceptable.
	/// </summary>
    public SslPolicyErrors AcceptablePolicyErrors { get; set; } = SslPolicyErrors.None;

	/// <summary>
	/// Retrieve or set server's expected name.
	/// This MUST match the Subject Alternative Name (SAN) or CN on the peer's (server's) leaf certificate,
	/// otherwise the TLS connection will fail.
	/// </summary>
    public string ServerName { get; set; }

	/// <summary>
	/// Retrieve or set the path to client certificate.
	/// </summary>
    public string CertPath { get; set; }

	/// <summary>
	/// Controls if TLS should indeed be used. Set to false to disable TLS
	/// on the connection.
	/// </summary>
    public bool Enabled { get; set; } = false;
}
