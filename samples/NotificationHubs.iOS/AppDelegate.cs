using System;
using System.Diagnostics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ByteSmith.WindowsAzure.Messaging;

namespace NotificationHubs.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		NotificationHubs_iOSViewController viewController;

		// notification hub declarations
		public static INotificationHub Hub { get; set; }
		public static Registration NativeRegistration { get; set; }

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			viewController = new NotificationHubs_iOSViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();
		
			Hub = new NotificationHub(
				"<your notification hub name>",
				"<your DefaultListenSharedAccessSignature connection string>");

			return true;
		}

		public async override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			try {
				NativeRegistration = await Hub.RegisterNativeAsync(deviceToken);
				new UIAlertView("Notification Hub Registration", "Registration ID: " + NativeRegistration.RegistrationId, null, "OK", null).Show();
			} catch (Exception ex) {
				new UIAlertView("Error registering push notifications", ex.Message, null, "OK", null).Show();
			}
		}

		public override void FailedToRegisterForRemoteNotifications (UIApplication application , NSError error)
		{
			new UIAlertView("Error registering push notifications", error.LocalizedDescription, null, "OK", null).Show();
		}

		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			var aps = userInfo [new NSString ("aps")] as NSDictionary;
			if (aps == null)
				return;

			var alert = aps [new NSString ("alert")] as NSString;
			if (alert == null)
				return;

			// show an alert
			new UIAlertView("Notification", alert.ToString(), null, "OK", null).Show();
		}
	}
}

