using Android.App;
using Android.Content;

namespace NotificationHubs.Android
{
	[BroadcastReceiver(Permission= "com.google.android.c2dm.permission.SEND")]
	[IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" }, Categories = new[] {"notificationHubsDemoApp" })]
	[IntentFilter(new[] { "com.google.android.c2dm.intent.REGISTRATION" }, Categories = new[] {"notificationHubsDemoApp" })]
	[IntentFilter(new[] { "com.google.android.gcm.intent.RETRY" }, Categories = new[] { "notificationHubsDemoApp"})]
	public class MyGCMBroadcastReceiver : BroadcastReceiver
	{
		const string TAG = "PushHandlerBroadcastReceiver";
		public override void OnReceive(Context context, Intent intent)
		{
			RemoteNotificationService.RunIntentInService(context, intent);
			SetResult(Result.Ok, null, null);
		}
	}
}