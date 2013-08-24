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
		private const string GcmNativeCreate = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><content type=\"application/xml\"><GcmRegistrationDescription xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\">{0}<GcmRegistrationId>{1}</GcmRegistrationId></GcmRegistrationDescription></content></entry>";
		private const string GcmTemplateCreate = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><content type=\"application/xml\"><GcmTemplateRegistrationDescription xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\">{0}<GcmRegistrationId>{1}</GcmRegistrationId><BodyTemplate><![CDATA[{2}]]></BodyTemplate><TemplateName>{3}</TemplateName></GcmTemplateRegistrationDescription></content></entry>";

		#region Private Methods

		private void ProcessXmlRegistration(Registration registration, XmlDocument registrationXml)
		{
			/*
            <entry a:etag="W/&quot;1&quot;" xmlns="http://www.w3.org/2005/Atom" xmlns:a="http://schemas.microsoft.com/ado/2007/08/dataservices/metadata">
                <id>https://blogdemo.servicebus.windows.net/myhub/registrations/3750328172024393254-9075900229351105338?api-version=2013-04</id>
                <title type="text">3750328172024393254-9075900229351105338</title>
                <published>2013-07-11T13:26:57Z</published>
                <updated>2013-07-11T13:26:57Z</updated>
                <link rel="self" href="https://blogdemo.servicebus.windows.net/myhub/registrations/3750328172024393254-9075900229351105338?api-version=2013-04" />
                <content type="application/xml">
                    <WindowsRegistrationDescription xmlns="http://schemas.microsoft.com/netservices/2010/10/servicebus/connect" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
                        <ETag>1</ETag>
                        <ExpirationTime>2013-10-09T13:26:57.704Z</ExpirationTime>
                        <RegistrationId>3750328172024393254-9075900229351105338</RegistrationId>
                        <ChannelUri>https://db3.notify.windows.com/?token=AgYAAAAMPgO7SXlTLbPKhpM34G%2beIqsZJYuT4pylZiloHUm3%2fVnWxotWlMdOLUhiHPeckuYZ6EAGDuQ2gznwsfkXLSUFSuzO3R1HmUuyy48PH%2f6yQoWUNc8zWIJaey0Cytef6p4%3d</ChannelUri>
                    </WindowsRegistrationDescription>
                </content>
            </entry>
             * 
            <entry a:etag="W/&quot;1&quot;" xmlns:a="http://schemas.microsoft.com/ado/2007/08/dataservices/metadata" xmlns="http://www.w3.org/2005/Atom">
                <id>https://blogdemo.servicebus.windows.net/myhub/tags/Both/registrations/280030996932335216-2707426160592076072?api-version=2013-04</id>
                <title type="text">280030996932335216-2707426160592076072</title>
                <published>2013-07-17T09:59:51Z</published>
                <updated>2013-07-17T09:59:51Z</updated>
                <link rel="self" href="https://blogdemo.servicebus.windows.net/myhub/tags/Both/registrations/280030996932335216-2707426160592076072?api-version=2013-04" />
                <content type="application/xml">
                    <AppleRegistrationDescription xmlns="http://schemas.microsoft.com/netservices/2010/10/servicebus/connect" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
                    <ETag>1</ETag>
                    <ExpirationTime>2013-10-14T11:36:35.805Z</ExpirationTime>
                    <RegistrationId>280030996932335216-2707426160592076072</RegistrationId>
                    <Tags>Both,Toast</Tags>
                    <DeviceToken>0D5066BE84F13FE7D1A935A9922C9BCA0890C9FDF57DDD1DB4FD8642331184C2</DeviceToken>
                    </AppleRegistrationDescription></content>
             </entry> 
            */
			DateTime dateTime;

			registration.Raw = registrationXml.OuterXml;

			var nodes = registrationXml.GetElementsByTagName("GcmRegistrationId");
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
		}

		private string BuildCreatePayload(Registration registration)
		{
			/*
            assuming:  
            tags,
            channelUri

            if template registration:
            assumes bodyTemplate AND wnsHeaders
            */

			string registrationPayload;

			var tagstring = String.Empty;
			if (registration.Tags != null)
			{
				tagstring = "<Tags>" + String.Join(",", registration.Tags) + "</Tags>";
			}

			if (!String.IsNullOrEmpty(registration.BodyTemplate))
			{
				registrationPayload = String.Format(
					GcmTemplateCreate,
					tagstring,
					registration.PnsToken,
					registration.BodyTemplate,
					registration.TemplateName);
			}
			else // native
			{
				registrationPayload = String.Format(
					GcmNativeCreate,
					tagstring,
					registration.PnsToken);
			}

			return registrationPayload;
		}

		public void StoreRegistrationToLocal(Registration registration)
		{
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var registrationsPath = System.IO.Path.Combine (documentsPath, 
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
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var registrationsPath = System.IO.Path.Combine (documentsPath, 
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
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var registrationsPath = System.IO.Path.Combine (documentsPath, 
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
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var filename = System.IO.Path.Combine(documentsPath, 
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
				if (String.IsNullOrEmpty(pnsToken))
					throw new ArgumentNullException("pnsToken");

				var serverUrl = _endpoint + '/' + Path + "/registrations?$filter=GcmRegistrationId eq '" + pnsToken + "'&api-version=2013-04";
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