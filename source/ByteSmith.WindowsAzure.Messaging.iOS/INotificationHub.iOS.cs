using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace ByteSmith.WindowsAzure.Messaging
{
	public partial interface INotificationHub
	{
		Task<Registration> RegisterNativeAsync (NSData deviceToken);
		Task<Registration> RegisterNativeAsync (NSData deviceToken, IEnumerable<string> tags);
		Task<Registration> RegisterTemplateAsync (NSData deviceToken, string template, string templateName);
		Task<Registration> RegisterTemplateAsync (NSData deviceToken, string bodyTemplate, string templateName, IEnumerable<string> tags);
		Task<Registration> RegisterTemplateAsync (NSData deviceToken, string bodyTemplate, string templateName, TimeSpan? apnsExpiry);
		Task<Registration> RegisterTemplateAsync (NSData deviceToken, string bodyTemplate, string templateName, IEnumerable<string> tags, TimeSpan? apnsExpiry);
		Task<Registration> RegisterTemplateAsync (string pnsToken, string bodyTemplate, string templateName, TimeSpan? apnsExpiry);
		Task<Registration> RegisterTemplateAsync (string pnsToken, string bodyTemplate, string templateName, IEnumerable<string> tags, TimeSpan? apnsExpiry);
		Task UnregisterAllAsync(NSData deviceToken);
	}
}