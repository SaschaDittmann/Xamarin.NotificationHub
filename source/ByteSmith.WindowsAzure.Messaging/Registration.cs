using System;
using System.Collections.Generic;

namespace ByteSmith.WindowsAzure.Messaging
{
    public partial class Registration
    {
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
		public IEnumerable<string> Tags { get; set; }
		public string BodyTemplate { get; set; }
		public string TemplateName { get; set; }
		public string Raw { get; set; }
    }
}
