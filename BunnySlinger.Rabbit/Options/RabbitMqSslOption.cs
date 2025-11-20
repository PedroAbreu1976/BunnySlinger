using System.Net.Security;
using System.Security.Authentication;

namespace BunnySlinger.Options;

public class RabbitMqSslOption
{
	public SslProtocols Version { get; set; } = SslProtocols.None;
	public SslPolicyErrors AcceptablePolicyErrors { get; set; } = SslPolicyErrors.None;
	public string ServerName { get; set; }
	public string CertPath { get; set; }
	public bool Enabled { get; set; } = false;
}
