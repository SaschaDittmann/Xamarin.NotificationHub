using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace NotificationHubs.iOS
{
	public partial class NotificationHubs_iOSViewController : UIViewController
	{
		public NotificationHubs_iOSViewController () : base ("NotificationHubs_iOSViewController", null)
		{
		}

		partial void RegisterDeviceButtonClick (MonoTouch.Foundation.NSObject sender)
		{
			UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (
				UIRemoteNotificationType.Alert);
		}

		partial void UnregisterDeviceButtonClick (MonoTouch.Foundation.NSObject sender)
		{
			try {
				if (AppDelegate.NativeRegistration != null)
				{
					AppDelegate.Hub.UnregisterNativeAsync();
					new UIAlertView("Notification Hub", "Unregistered successfully", null, "OK", null).Show();
				}
				else
					new UIAlertView("Notification Hub", "Device not registered", null, "OK", null).Show();
			} catch (Exception ex) {
				new UIAlertView("Error unregistering push notifications", ex.Message, null, "OK", null).Show();
			}
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return (toInterfaceOrientation == UIInterfaceOrientation.Portrait);
		}
	}
}

