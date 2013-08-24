using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ByteSmith.WindowsAzure.Messaging
{
	[Serializable]
    public partial class Registration
    {
		public Registration ()
		{
		}

		public Registration (string pnsToken, string notificationHubPath) 
		{
			PnsToken = pnsToken;
			NotificationHubPath = notificationHubPath;
		}

        public string PnsToken { get; set; }
		public string NotificationHubPath { get; set; }
        public string ETag { get; set; }
		public DateTime? PublishedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string RegistrationId { get; set; }
		[XmlIgnore]
		public IEnumerable<string> Tags { get; set; }
		[XmlArray("Tags"), XmlArrayItem("Tag"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string[] TagsSurrogate { 
			get { 
				return Tags.ToArray(); 
			} 
			set { 
				Tags = value; 
			} 
		}
		public string BodyTemplate { get; set; }
		public string TemplateName { get; set; }
		public string Raw { get; set; }
    }
}
