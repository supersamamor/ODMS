using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record DataSourceState : BaseEntity
{
	public string Name { get; init; } = "";
	public string? ServerAddress { get; init; }
	public string? DatabaseName { get; init; }
	public string? AuthenticationType { get; init; }
	public string? Username { get; init; }
	public string? PasswordEncrypted { get; init; }
	public string? ConnectionStringEncrypted { get; init; }
	public string? Description { get; init; }
	public bool IsActive { get; init; }
	
	
	
}
