using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using MonoTouch.Foundation;

namespace ByteSmith.WindowsAzure.Messaging
{
	public partial class NotificationHub
	{
		private const string ApnsNativeCreate = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><content type=\"application/xml\"><AppleRegistrationDescription xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\">{0}<DeviceToken>{1}</DeviceToken></AppleRegistrationDescription></content></entry>";
		private const string ApnsTemplateCreate = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><content type=\"application/xml\"><AppleTemplateRegistrationDescription xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\">{0}<DeviceToken>{1}</DeviceToken><BodyTemplate><![CDATA[{2}]]></BodyTemplate><TemplateName>{3}</TemplateName>{4}</AppleTemplateRegistrationDescription></content></entry>";

		#region Public Methods

		public Task<Registration> RegisterNativeAsync(NSData deviceToken)
		{
			return RegisterNativeAsync(deviceToken.Description.Trim('<', '>').Replace(" ",""));
		}

		public Task<Registration> RegisterNativeAsync(NSData deviceToken, IEnumerable<string> tags)
		{
			return RegisterNativeAsync(deviceToken.Description.Trim('<', '>').Replace(" ",""), tags);
		}

		public Task<Registration> RegisterTemplateAsync(NSData deviceToken, string bodyTemplate, string templateName)
		{
			return RegisterTemplateAsync(deviceToken.Description.Trim('<', '>').Replace(" ",""), bodyTemplate, templateName);
		}

		public Task<Registration> RegisterTemplateAsync(NSData deviceToken, string bodyTemplate, string templateName, TimeSpan? apnsExpiry)
		{
			return RegisterTemplateAsync(deviceToken.Description.Trim('<', '>').Replace(" ",""), bodyTemplate, templateName, apnsExpiry);
		}

		public Task<Registration> RegisterTemplateAsync(NSData deviceToken, string bodyTemplate, string templateName, IEnumerable<string> tags)
		{
			return RegisterTemplateAsync(deviceToken.Description.Trim('<', '>').Replace(" ", ""), bodyTemplate, templateName, tags);
		}

		public Task<Registration> RegisterTemplateAsync(NSData deviceToken, string bodyTemplate, string templateName, IEnumerable<string> tags, TimeSpan? apnsExpiry)
		{
			return RegisterTemplateAsync(deviceToken.Description.Trim('<', '>').Replace(" ", ""), bodyTemplate, templateName, tags, apnsExpiry);
		}

		public Task<Registration> RegisterTemplateAsync(string pnsToken, string bodyTemplate, string templateName, TimeSpan? apnsExpiry)
		{
			return Register(new Registration(pnsToken, Path)
			{
				ApnsExpiry = apnsExpiry,
				BodyTemplate = bodyTemplate
			}, templateName);
		}

		public Task<Registration> RegisterTemplateAsync(string pnsToken, string bodyTemplate, string templateName, IEnumerable<string> tags, TimeSpan? apnsExpiry)
		{
			return Register(new Registration(pnsToken, Path)
			{
				Tags = tags,
				ApnsExpiry = apnsExpiry,
				BodyTemplate = bodyTemplate
			}, templateName);
		}

		public Task UnregisterAllAsync(NSData deviceToken)
		{
			return UnregisterAllAsync(deviceToken.Description.Trim ('<', '>').Replace (" ", ""));
		}

		#endregion

		#region Private Methods

		private void ProcessXmlRegistration(Registration registration, XmlDocument registrationXml)
		{
			DateTime dateTime;
			double expiry;

			registration.Raw = registrationXml.OuterXml;

			var nodes = registrationXml.GetElementsByTagName("DeviceToken");
			if (nodes.Count > 0) registration.PnsToken = nodes[0].InnerText;

			nodes = registrationXml.GetElementsByTagName("ETag");
			if (nodes.Count > 0) registration.ETag = nodes[0].InnerText;

			nodes = registrationXml.GetElementsByTagName("published");
			if (nodes.Count > 0 && DateTime.TryParse(nodes[0].InnerText, out dateTime))
				registration.PublishedAt = dateTime;

			nodes = registrationXml.GetElementsByTagName("updated");
			if (nodes.Count > 0 && DateTime.TryParse(nodes[0].InnerText, out dateTime))
				registration.UpdatedAt = dateTime;

			nodes = registrationXml.GetElementsByTagName("ExpirationTime");
			if (nodes.Count > 0 && DateTime.TryParse(nodes[0].InnerText, out dateTime))
				registration.ExpiresAt = dateTime;

			nodes = registrationXml.GetElementsByTagName("RegistrationId");
			if (nodes.Count > 0) registration.RegistrationId = nodes[0].InnerText;

			nodes = registrationXml.GetElementsByTagName("Tags");
			registration.Tags = nodes.Count > 0 ? nodes[0].InnerText.Split(',') : null;

			nodes = registrationXml.GetElementsByTagName("BodyTemplate");
			registration.BodyTemplate = nodes.Count > 0 ? nodes[0].InnerText : null;

			nodes = registrationXml.GetElementsByTagName("TemplateName");
			registration.TemplateName = nodes.Count > 0 ? nodes[0].InnerText : null;

			nodes = registrationXml.GetElementsByTagName("Expiry");
			if (nodes.Count > 0 && double.TryParse(nodes[0].InnerText, out expiry))
				registration.ApnsExpiry = TimeSpan.FromSeconds(expiry);
			else
				registration.ApnsExpiry = null;
		}

		private string BuildCreatePayload(Registration registration)
		{
			string registrationPayload;

			var tagstring = String.Empty;
			if (registration.Tags != null)
			{
				tagstring = "<Tags>" + String.Join(",", registration.Tags) + "</Tags>";
			}

			if (!String.IsNullOrEmpty(registration.BodyTemplate))
			{
				registrationPayload = String.Format(
					ApnsTemplateCreate,
					tagstring,
					registration.PnsToken,
					registration.BodyTemplate,
					registration.TemplateName,
					registration.ApnsExpiry.HasValue
						? "<Expiry>" + registration.ApnsExpiry.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture) + "</Expiry>"
						: "");
			}
			else // native
			{
				registrationPayload = String.Format(
					ApnsNativeCreate,
					tagstring,
					registration.PnsToken);
			}

			return registrationPayload;
		}

		#endregion

		#region API Calls

		private async Task<List<Registration>> ReadAllRegistrations(string pnsToken)
		{
			try
			{
				if (String.IsNullOrEmpty(pnsToken))
					throw new ArgumentNullException("pnsToken");

				var serverUrl = _endpoint + '/' + Path + "/registrations?$filter=DeviceToken eq '" + pnsToken + "'&api-version=2013-04";
				Debug.WriteLine("Reading All Registrations by token (" + pnsToken + ") ...");
				Debug.WriteLine("url:" + serverUrl);

				var registrationsXml = new XmlDocument();
				using (var client = new WebClient())
				{
					AddWebClientHeaders(client, serverUrl);
					var result = await client.DownloadStringTaskAsync(serverUrl);
					Debug.WriteLine("registration result:" + result);
					registrationsXml.LoadXml(result);
				}

				return ProcessXmlRegistrations(pnsToken, registrationsXml);
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