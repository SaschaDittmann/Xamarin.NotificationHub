using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ByteSmith.WindowsAzure.Messaging
{
	public partial class NotificationHub
	{
		#region Private Methods

		private void ProcessXmlRegistration(Registration registration, XmlDocument registrationXml)
		{
			DateTime dateTime;

			registration.Raw = registrationXml.OuterXml;

            var nodes = registrationXml.GetElementsByTagName("ETag");
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
		}

		private string BuildCreatePayload(Registration registration)
		{
			return String.Empty;
		}

		public void StoreRegistrationToLocal(Registration registration)
		{
			var registrationsPath = System.IO.Path.Combine (Environment.CurrentDirectory, 
			                                                "NotificationHubs", 
			                                                _endpoint.Replace ("https://", ""),
			                                                Path);
			if (!Directory.Exists (registrationsPath))
				Directory.CreateDirectory (registrationsPath);
			var filename = System.IO.Path.Combine(registrationsPath, 
			    String.IsNullOrEmpty(registration.TemplateName)
					? "$native.xml"
					: registration.TemplateName + ".xml");

			using (TextWriter writer = new StreamWriter(filename)) {
				var serializer = new XmlSerializer(typeof(Registration));
				serializer.Serialize(writer, registration);
			}
		}

		public Registration LoadRegistrationFromLocal(string templateName)
		{
			var registrationsPath = System.IO.Path.Combine (Environment.CurrentDirectory, 
			                                                "NotificationHubs", 
			                                                _endpoint.Replace ("https://", ""),
			                                                Path);
			if (!Directory.Exists (registrationsPath))
				Directory.CreateDirectory (registrationsPath);
			var filename = System.IO.Path.Combine(registrationsPath, 
				String.IsNullOrEmpty(templateName)
					? "$native.xml"
					: templateName + ".xml");

			if (!File.Exists (filename))
				return null;

			using (TextReader reader = new StreamReader(filename)) {
				var serializer = new XmlSerializer(typeof(Registration));
				var localRegistration = (Registration)serializer.Deserialize(reader);
				return !localRegistration.ExpiresAt.HasValue || localRegistration.ExpiresAt.Value <= DateTime.UtcNow
					? null 
					: localRegistration;
			}
		}

		public List<Registration> LoadAllRegistrationsFromLocal() {
			var registrations = new List<Registration> ();
			var registrationsPath = System.IO.Path.Combine (Environment.CurrentDirectory, 
			                                                "NotificationHubs", 
			                                                _endpoint.Replace ("https://", ""),
			                                                Path);
			if (!Directory.Exists (registrationsPath))
				Directory.CreateDirectory (registrationsPath);
			var filenames = Directory.EnumerateFiles(registrationsPath, "*.xml", SearchOption.TopDirectoryOnly);
			foreach (var filename in filenames) {
				var registration = LoadRegistrationFromLocal (
					System.IO.Path.GetFileNameWithoutExtension (filename));

				if (registration != null)
					registrations.Add (registration);
			}
			return registrations;
		}

		public void DeleteLocalRegistration (string templateName)
		{
			var filename = System.IO.Path.Combine(Environment.CurrentDirectory, 
				String.IsNullOrEmpty(templateName)
					? "$native.xml"
					: templateName + ".xml");

			if (File.Exists (filename))
				File.Delete(filename);
		}

		#endregion

		#region API Calls

		private async Task<List<Registration>> ReadAllRegistrations(string pnsToken)
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

		#endregion
	}
}