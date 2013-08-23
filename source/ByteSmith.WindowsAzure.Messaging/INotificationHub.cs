using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ByteSmith.WindowsAzure.Messaging
{
	public partial interface INotificationHub : IDisposable
	{
		string Connection { get; set; }
		string Path { get; set; }
		Task<Registration> RegisterAsync (Registration registration);
		Task<Registration> RegisterNativeAsync (string pnsToken);
		Task<Registration> RegisterNativeAsync (string pnsToken, IEnumerable<string> tags);
		Task<Registration> RegisterTemplateAsync (string pnsToken, string template, string templateName);
		Task<Registration> RegisterTemplateAsync (string pnsToken, string bodyTemplate, string templateName, IEnumerable<string> tags);
		Task UnregisterAllAsync(string pnsToken);
		Task UnregisterAsync(Registration registration);
		Task UnregisterNativeAsync();
		Task UnregisterTemplateAsync(string templateName);

		Task<Registration> GetRegistrationAsync(Registration registration);
		Task<Registration> GetRegistrationByIdAsync(string registrationId);
		Task<List<Registration>> GetRegistrationsAsync();
		Task<List<Registration>> GetRegistrationsAsync(string pnsToken);
		Task<List<Registration>> GetRegistrationsByTagAsync(string tag);

		Task SendNotificationAsync(NotificationHubMessage notification);
		Task SendWindowsNativeNotificationAsync(string windowsNativePayload);
		Task SendWindowsNativeNotificationAsync(string windowsNativePayload, string tag);
		Task SendMpnsNativeNotificationAsync(string nativePayload);
		Task SendMpnsNativeNotificationAsync(string nativePayload, string tag);
		Task SendAppleNativeNotificationAsync(string jsonPayload);
		Task SendAppleNativeNotificationAsync(string jsonPayload, string tag);
		Task SendGcmNativeNotificationAsync(string jsonPayload);
		Task SendGcmNativeNotificationAsync(string jsonPayload, string tag);
		Task SendTemplateNotificationAsync(Dictionary<string, string> properties);
		Task SendTemplateNotificationAsync(Dictionary<string, string> properties, string tag);
	}
}

