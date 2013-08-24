using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ByteSmith.WindowsAzure.Messaging
{
	public partial interface INotificationHub : IDisposable
	{
		void StoreRegistrationToLocal (Registration registration);
		Registration LoadRegistrationFromLocal (string templateName);
		List<Registration> LoadAllRegistrationsFromLocal ();
		void DeleteLocalRegistration (string templateName);
	}
}

