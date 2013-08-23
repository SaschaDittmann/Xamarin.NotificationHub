using System.Collections.Generic;

namespace ByteSmith.WindowsAzure.Messaging
{
	public class NotificationHubMessage
	{
		public string Body { get; set; }
		public string ContentType { get; set; }
		public IDictionary<string, string> Headers { get; set; }
		public string Tag { get; set; }
	}
}