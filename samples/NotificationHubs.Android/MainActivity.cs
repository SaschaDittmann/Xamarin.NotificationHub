using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;

namespace NotificationHubs.Android
{
	[Activity (Label = "ByteSmith.BlogDemo.Android", MainLauncher = true)]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			var registerButton = FindViewById<Button> (Resource.Id.registerButton);
			var unregisterButton = FindViewById<Button> (Resource.Id.unregisterButton);
			
			registerButton.Click += delegate {
				const string senders = "<Google Cloud Messaging Sender ID>";
				var intent = new Intent("com.google.android.c2dm.intent.REGISTER");
				intent.SetPackage("com.google.android.gsf");
				intent.PutExtra("app", PendingIntent.GetBroadcast(this, 0, new Intent(), 0));
				intent.PutExtra("sender", senders);
				StartService(intent);
			};

			unregisterButton.Click += delegate {
				var intent = new Intent("com.google.android.c2dm.intent.UNREGISTER");
				intent.PutExtra("app", PendingIntent.GetBroadcast(this, 0, new Intent(), 0));
				StartService(intent);
			};
		}
	}
}