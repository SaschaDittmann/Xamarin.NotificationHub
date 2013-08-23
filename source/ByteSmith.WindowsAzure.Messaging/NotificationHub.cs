using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.Xml;

namespace ByteSmith.WindowsAzure.Messaging
{
	public partial class NotificationHub : INotificationHub
	{
		private readonly Dictionary<string, Registration> _registrations;

		private string _endpoint;
		private string _sasName;
		private string _sasKey;

		#region Public Properties

		public string Connection
		{
			get
			{
				return String.Format(
					"Endpoint={0};SharedAccessKeyName={1};SharedAccessKey={2}",
					_endpoint,
					_sasName,
					_sasKey);
			}
			set
			{
				foreach (var arg in value.Split(';'))
				{
					var pos = arg.IndexOf('=');
					if (pos >= 0)
					{
						switch (arg.Substring(0, pos).Trim().ToLower())
						{
							case "endpoint":
							    _endpoint = arg.Substring(pos + 1).Trim().ToLower().Replace("sb://", "https://").TrimEnd('/');
							    break;
							case "sharedaccesskeyname":
							    _sasName = arg.Substring(pos + 1).Trim();
							    break;
							case "sharedaccesskey":
							    _sasKey = arg.Substring(pos + 1).Trim();
							    break;
						}
					}
				}
                if (String.IsNullOrEmpty(_endpoint) || String.IsNullOrEmpty(_sasName) || String.IsNullOrEmpty(_sasKey))
			    {
			        throw new ArgumentException("Invalid Connection String");
			    }
			}
		}
		public string Path { get; set; }

		#endregion

		#region Public Methods

		public NotificationHub(string hubPath, string connectionString)
		{
			Connection = connectionString;
			Path = hubPath;

			_registrations = new Dictionary<string, Registration>();
		}

		public NotificationHub(string endpoint, string sasName, string sasKey, string hubPath)
		{
			Path = hubPath;

			_endpoint = endpoint.ToLower().Replace("sb://", "https://").TrimEnd('/');
			_sasName = sasName;
			_sasKey = sasKey;

			_registrations = new Dictionary<string, Registration>();
		}

		public Task<Registration> RegisterAsync(Registration registration)
		{
			return Register(registration, "$native");
		}

		public Task<Registration> RegisterNativeAsync(string pnsToken)
		{
			return Register(new Registration(pnsToken, Path), "$native");
		}

		public Task<Registration> RegisterNativeAsync(string pnsToken, IEnumerable<string> tags)
		{
			return Register(new Registration(pnsToken, Path) { Tags = tags }, "$native");
		}

		public Task<Registration> RegisterTemplateAsync(string pnsToken, string bodyTemplate, string templateName)
		{
			return Register(new Registration(pnsToken, Path) 
			{ 
				BodyTemplate = bodyTemplate, 
				TemplateName = templateName 
			}, templateName);
		}

		public Task<Registration> RegisterTemplateAsync(string pnsToken, string bodyTemplate, string templateName, IEnumerable<string> tags)
		{
			return Register(new Registration(pnsToken, Path)
			{
				Tags = tags,
				BodyTemplate = bodyTemplate,
				TemplateName = templateName 
			}, templateName);
		}

		public Task UnregisterAllAsync(string pnsToken)
		{
			var tasks = (from registration in _registrations 
                         where registration.Value.PnsToken == pnsToken 
                         select UnregisterAsync(registration.Value)).ToList();
		    return Task.WhenAll (tasks);
		}

		public Task UnregisterAsync(Registration registration)
		{
			try
			{
				if (String.IsNullOrEmpty(registration.RegistrationId))
					throw new ArgumentNullException("registration");
				if (String.IsNullOrEmpty(registration.RegistrationId))
					throw new ArgumentException("RegistrationId missing.", "registration");

				return DeleteRegistration(registration);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				throw;
			}
		}

		public Task UnregisterNativeAsync()
		{
			return Unregister("$native", null);
		}

		public Task UnregisterTemplateAsync(string templateName)
		{
			return Unregister(templateName, null);
		}

		public Task<Registration> GetRegistrationAsync(Registration registration)
		{
			return ReadRegistration(registration);
		}

		public Task<Registration> GetRegistrationByIdAsync(string registrationId)
		{
			return ReadRegistration(new Registration("", Path) { RegistrationId = registrationId });
		}

		public Task<List<Registration>> GetRegistrationsAsync()
		{
			return ReadAllRegistrations();
		}

		public Task<List<Registration>> GetRegistrationsAsync(string pnsToken)
		{
			return ReadAllRegistrations(pnsToken);
		}

		public Task<List<Registration>> GetRegistrationsByTagAsync(string tag)
		{
			return ReadAllRegistrationsByTag(tag);
		}

		public Task SendNotificationAsync(NotificationHubMessage notification)
		{
			return SendNotification (notification);
		}

		public Task SendWindowsNativeNotificationAsync(string windowsNativePayload)
		{
			return SendNotificationAsync (new NotificationHubMessage {
				Body = windowsNativePayload,
				ContentType = "application/octet-stream", 
				Headers = new Dictionary<string,string> { {"ServiceBusNotification-Format", "windows"} },
			});
		}

		public Task SendWindowsNativeNotificationAsync(string windowsNativePayload, string tag)
		{
			return SendNotificationAsync (new NotificationHubMessage {
				Body = windowsNativePayload,
				ContentType = "application/octet-stream", 
				Headers = new Dictionary<string,string> { {"ServiceBusNotification-Format", "windows"} },
				Tag = tag,
			});
		}

		public Task SendMpnsNativeNotificationAsync(string nativePayload)
		{
			return SendNotificationAsync (new NotificationHubMessage {
				Body = nativePayload,
				ContentType = "application/xml;charset=utf-8", 
				Headers = new Dictionary<string,string> { {"ServiceBusNotification-Format", "windowsphone"} },
			});
		}

		public Task SendMpnsNativeNotificationAsync(string nativePayload, string tag)
		{
			return SendNotificationAsync (new NotificationHubMessage {
				Body = nativePayload,
				ContentType = "application/xml;charset=utf-8", 
				Headers = new Dictionary<string,string> { {"ServiceBusNotification-Format", "windowsphone"} },
				Tag = tag,
			});
		}

		public Task SendAppleNativeNotificationAsync(string jsonPayload)
		{
			return SendNotificationAsync (new NotificationHubMessage {
				Body = jsonPayload,
				ContentType = "application/json;charset=utf-8", 
				Headers = new Dictionary<string,string> { {"ServiceBusNotification-Format", "apple"} },
			});
		}

		public Task SendAppleNativeNotificationAsync(string jsonPayload, string tag)
		{
			return SendNotificationAsync (new NotificationHubMessage {
				Body = jsonPayload,
				ContentType = "application/json;charset=utf-8", 
				Headers = new Dictionary<string,string> { {"ServiceBusNotification-Format", "apple"} },
				Tag = tag,
			});
		}

		public Task SendGcmNativeNotificationAsync(string jsonPayload)
		{
			return SendNotificationAsync (new NotificationHubMessage {
				Body = jsonPayload,
				ContentType = "application/json;charset=utf-8", 
				Headers = new Dictionary<string,string> { {"ServiceBusNotification-Format", "gcm"} },
			});
		}

		public Task SendGcmNativeNotificationAsync(string jsonPayload, string tag)
		{
			return SendNotificationAsync (new NotificationHubMessage {
				Body = jsonPayload,
				ContentType = "application/json;charset=utf-8", 
				Headers = new Dictionary<string,string> { {"ServiceBusNotification-Format", "gcm"} },
				Tag = tag,
			});
		}

		public Task SendTemplateNotificationAsync(Dictionary<string, string> properties)
		{
			var jsonPayload = new StringBuilder ("{");

			foreach(var property in properties)
			{
				jsonPayload.AppendFormat ("\"{0}\":\"{1}\",", property.Key, property.Value);
			}
			jsonPayload.Append ("}");

			return SendNotificationAsync (new NotificationHubMessage {
				Body = jsonPayload.ToString(),
				ContentType = "application/json;charset=utf-8", 
				Headers = new Dictionary<string,string> { {"ServiceBusNotification-Format", "template"} },
			});
		}

		public Task SendTemplateNotificationAsync(Dictionary<string, string> properties, string tag)
		{
			var jsonPayload = new StringBuilder ("{");

			foreach(var property in properties)
			{
				jsonPayload.AppendFormat ("\"{0}\":\"{1}\",", property.Key, property.Value);
			}
			jsonPayload.Append ("}");

			return SendNotificationAsync (new NotificationHubMessage {
				Body = jsonPayload.ToString(),
				ContentType = "application/json;charset=utf-8", 
				Headers = new Dictionary<string,string> { {"ServiceBusNotification-Format", "template"} },
				Tag = tag,
			});
		}

		public void Dispose()
		{
		}

		#endregion

		#region Private Methods

		private async Task<Registration> Register(Registration registration, string name)
		{
			//if (_registrations.Count == 0) {
			//	var regs = await ReadAllRegistrations (registration.PnsToken);
			//	foreach (var reg in regs) {
			//		var regKeyExisting = String.IsNullOrEmpty(reg.TemplateName)
			//			? _endpoint + '/' + Path + "/app/$native"
			//			: _endpoint + '/' + Path + "/app/" + reg.TemplateName;
			//		_registrations.Add(regKeyExisting, reg);
			//	}
			//}

			var regKey = _endpoint + '/' + Path + "/app/" + name;

			if (_registrations.ContainsKey(regKey))
				registration = await UpdateRegistration(_registrations[regKey]);
			else
				registration = await CreateRegistration(registration);

			_registrations[regKey] = registration;

			return registration;
		}

		private async Task Unregister(string name, string tileId)
		{
			var tileKey = String.IsNullOrEmpty(tileId)
				? "application"
					: tileId;
			var regKey = _endpoint + '/' + Path + '/' + tileKey + '/' + name;
			if (!_registrations.ContainsKey(regKey))
				return;

			await DeleteRegistration(_registrations[regKey]);

			_registrations.Remove(regKey);
		}

		private List<Registration> ProcessXmlRegistrations(string pnsToken, XmlDocument registrationsXml)
		{
			var registrations = new List<Registration>();

			var registrationXml = new XmlDocument();
			foreach (XmlNode entry in registrationsXml.GetElementsByTagName("entry"))
			{
				var registration = new Registration(pnsToken, Path);
				registrationXml.LoadXml(entry.OuterXml);
				ProcessXmlRegistration(registration, registrationXml);
				registrations.Add(registration);
			}

			return registrations;
		}

		private void AddWebClientHeaders(WebClient client, string serverUrl)
		{
			var token = SASTokenGenerator.GetSASToken(serverUrl, _sasName, _sasKey, TimeSpan.FromMinutes(60));
			client.Headers.Add("Content-Type", "application/atom+xml;type=entry;charset=utf-8");
			client.Headers.Add("Authorization", token);
			client.Headers.Add("x-ms-version", "2013-04");
		}

		#endregion

		#region API Calls

		private async Task SendNotification(NotificationHubMessage notification)
		{
			try
			{
				if (notification == null)
					throw new ArgumentNullException("notification");
				if (String.IsNullOrEmpty(notification.Body))
                    throw new ArgumentException("The Body property is empty", "notification");
				if (String.IsNullOrEmpty(notification.ContentType))
                    throw new ArgumentException("The ContentType property is empty", "notification");
				var serverUrl = _endpoint + '/' + Path + "/messages?api-version=2013-04";
				Debug.WriteLine("Sending Notification...");
				Debug.WriteLine("url:" + serverUrl);
				Debug.WriteLine("payload:" + notification.Body);

				using (var client = new WebClient())
				{
					var token = SASTokenGenerator.GetSASToken(serverUrl, _sasName, _sasKey, TimeSpan.FromMinutes(60));
					client.Headers.Add("Authorization", token);
					client.Headers.Add("Content-Type", notification.ContentType);
					if (!String.IsNullOrEmpty(notification.Tag))
						client.Headers.Add("ServiceBusNotification-Tags", notification.Tag);
					foreach(var header in notification.Headers)
						client.Headers.Add(header.Key, header.Value);

					var result = await client.UploadStringTaskAsync(serverUrl, "POST", notification.Body);
					Debug.WriteLine("registration result:" + result);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				throw;
			}
		}

		private async Task<Registration> CreateRegistration(Registration registration)
		{
			try
			{
				if (registration == null)
					throw new ArgumentNullException("registration");

				var registrationPayload = BuildCreatePayload(registration);
				var serverUrl = _endpoint + '/' + Path + "/registrations?api-version=2013-04";
				Debug.WriteLine("Creating Registration...");
				Debug.WriteLine("url:" + serverUrl);
				Debug.WriteLine("payload:" + registrationPayload);

				var registrationXml = new XmlDocument();
				//string location;

				using (var client = new WebClient())
				{
					AddWebClientHeaders(client, serverUrl);
					var result = await client.UploadStringTaskAsync(serverUrl, "POST", registrationPayload);
					Debug.WriteLine("registration result:" + result);
					registrationXml.LoadXml(result);
					//location = client.ResponseHeaders["Content-Location"];
				}

				ProcessXmlRegistration(registration, registrationXml);

				return registration;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				throw;
			}
		}

		private async Task<Registration> ReadRegistration(Registration registration)
		{
			try
			{
				if (String.IsNullOrEmpty(registration.RegistrationId))
					throw new ArgumentNullException("registration");
				if (String.IsNullOrEmpty(registration.RegistrationId))
					throw new ArgumentException("RegistrationId missing.", "registration");

				var serverUrl = _endpoint + '/' + Path + "/registrations/" + registration.RegistrationId + "?api-version=2013-04";
				Debug.WriteLine("Reading Registration (" + registration.RegistrationId + ") ...");
				Debug.WriteLine("url:" + serverUrl);

				var registrationXml = new XmlDocument();
				using (var client = new WebClient())
				{
					AddWebClientHeaders(client, serverUrl);
					var result = await client.DownloadStringTaskAsync(serverUrl);
					Debug.WriteLine("registration result:" + result);
					registrationXml.LoadXml(result);
				}

				ProcessXmlRegistration(registration, registrationXml);

				return registration;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				throw;
			}
		}

		private async Task<Registration> UpdateRegistration(Registration registration)
		{
			try
			{
				if (String.IsNullOrEmpty(registration.RegistrationId))
					throw new ArgumentNullException("registration");
				if (String.IsNullOrEmpty(registration.RegistrationId))
					throw new ArgumentException("RegistrationId missing.", "registration");

				var registrationPayload = BuildCreatePayload(registration);
				var serverUrl = _endpoint + '/' + Path + "/registrations/" + registration.RegistrationId + "?api-version=2013-04";
				Debug.WriteLine("Updating Registration (" + registration.RegistrationId + ") ...");
				Debug.WriteLine("url:" + serverUrl);
				Debug.WriteLine("payload:" + registrationPayload);

				var registrationXml = new XmlDocument();
				//string location;
				using (var client = new WebClient())
				{
					AddWebClientHeaders(client, serverUrl);
					client.Headers.Add("If-Match", String.IsNullOrEmpty(registration.ETag) ? "*" : registration.ETag);
					var result = await client.UploadStringTaskAsync(serverUrl, "PUT", registrationPayload);
					Debug.WriteLine("registration result:" + result);
					registrationXml.LoadXml(result);
					//location = client.ResponseHeaders["Content-Location"];
				}

				ProcessXmlRegistration(registration, registrationXml);

				return registration;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				throw;
			}
		}

		private async Task DeleteRegistration(Registration registration)
		{
			try
			{
				if (String.IsNullOrEmpty(registration.RegistrationId))
					throw new ArgumentNullException("registration");
				if (String.IsNullOrEmpty(registration.RegistrationId))
					throw new ArgumentException("RegistrationId missing.", "registration");

				var serverUrl = _endpoint + '/' + Path + "/registrations/" + registration.RegistrationId + "?api-version=2013-04";
				Debug.WriteLine("url:" + serverUrl);

				using (var client = new WebClient())
				{
					AddWebClientHeaders(client, serverUrl);
					client.Headers.Add(
						"If-Match",
						String.IsNullOrEmpty(registration.ETag) ? "*" : registration.ETag);
					await client.UploadStringTaskAsync(serverUrl, "DELETE", "");
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				throw;
			}
		}

		private async Task<List<Registration>> ReadAllRegistrations()
		{
			try
			{
				var serverUrl = _endpoint + '/' + Path + "/registrations?api-version=2013-04";
				Debug.WriteLine("Reading All Registrations...");
				Debug.WriteLine("url:" + serverUrl);

				var registrationsXml = new XmlDocument();
				using (var client = new WebClient())
				{
					AddWebClientHeaders(client, serverUrl);
					var result = await client.DownloadStringTaskAsync(serverUrl);
					Debug.WriteLine("registration result:" + result);
					registrationsXml.LoadXml(result);
				}

				return ProcessXmlRegistrations("", registrationsXml);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				throw;
			}
		}

		private async Task<List<Registration>> ReadAllRegistrationsByTag(string tag)
		{
			try
			{
				var serverUrl = _endpoint + '/' + Path + "/tags/" + tag + "/registrations?api-version=2013-04";
				Debug.WriteLine("Reading All Registrations by Tag (" + tag + ") ...");
				Debug.WriteLine("url:" + serverUrl);

				var registrationsXml = new XmlDocument();
				using (var client = new WebClient())
				{
					AddWebClientHeaders(client, serverUrl);
					var result = await client.DownloadStringTaskAsync(serverUrl);
					Debug.WriteLine("registration result:" + result);
					registrationsXml.LoadXml(result);
				}

				return ProcessXmlRegistrations("", registrationsXml);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				throw;
			}
		}

		#endregion
	}
}