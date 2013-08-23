#Windows Azure Notification Hubs

**Notification Hubs provide a highly scalable, cross-platform push notification infrastructure that enables you to either broadcast push notifications to millions of users at once or tailor notifications to individual users.**

On every mobile platform, push notifications are a critical element of any application. Push Notifications are simply the most immediate means through which to engage and empower your users. Building and maintaining the infrastructure for a push notification system capable of reaching millions of users within minutes, however, is far from simple. On your own, delivering millions of push notifications within minutes would require tens of virtual machines running in parallel. We created Notification Hubs to give developers an easy and reliable way to reach their users on any platform and from any connected application backend.

Use Notification Hubs to:

##Broadcast cross-platform push notifications to millions of devices in minutes
Notification Hubs supply a common API to send push notifications to a variety of mobile platforms, including Windows Store, Windows Phone, iOS and Android. You can choose to send platform-specific notifications or broadcast a single platform-agnostic notification to all users. A few lines of code gives you the power to reach either all devices on a single platform or all iOS, Android and Windows devices at once.

Notification Hubs send out push notifications to millions of users within minutes, not hours. That makes this service a particularly good partner for when speed matters most—such as with breaking news.

##Send notifications from any backend
Notification Hubs can be used with any connected application—whether it’s built on Virtual Machines, Cloud Services, Web Sites, or Mobile Services. This makes it easy to update any of your mobile apps right away and start engaging your users on their terms.

##Target content to specific user segments
With Notification Hubs, you not only have the ability to broadcast notifications to all your users at once (regardless of their mobile platform). You also have the ability to subscribe users to any number of tags when you register them with a Notification Hub. Those tags give you an easy way to define and target user segments based on activity, interests, location, etc. with a single API call. By using those tags effectively, you never have to store and mange device tokens or Ids in your app’s backend in order to route notifications to particular users.

##Use templates to tailor each user’s notifications
Templates provide a way for developers to specify the exact format of the notification that each user receives based on each of their preferences. By using templates, there is no need to store the localization settings for each of your customers or to create hundreds of tags. You just need to register the templates that specify the correct language with a Notification Hub and send a single message with all the localized content. Once your Notification Hub receives that single message, it will extract the correct localized message for each targeted user from the message.

##Achieve extreme scale
Notification Hubs are optimized for massive scale. With Notification Hubs, you can quickly scale to millions of devices and billions of push notifications without ever having to re-architect or shard your application. The Notification Hub you configure for a given application will automatically handle the pub/sub scale-out infrastructure necessary to scale your message to every active device with incredibly low latency. All it takes is one message from your connect app’s backend to the Notification Hub and millions of push notifications will be fired off to your users.

Notification Hubs are backed by a high availability service level agreement. To start sending push notifications to every user on every device, you will need a Windows Azure account. Sign up for the free trial [here](http://www.windowsazure.com/en-us/pricing/free-trial/).

##Configure your app to use a Notification Hub

In order to register to a notification hub to receive push notifications, use the **NotificationHub** client class. 
This class must be a singleton in your client app.

###iOS
One way to instantiate a singleton instance is to store it as a parameter in the **AppDelegate** class (defined in the file AppDelegate.cs), as shown here:

```csharp
using ByteSmith.WindowsAzure.Messaging;
...

public static INotificationHub Hub { get; set; }

public override bool FinishedLaunching (UIApplication app, NSDictionary options)
{
	window = new UIWindow (UIScreen.MainScreen.Bounds);

	viewController = new ByteSmith_BlogDemo_iOSViewController ();
	window.RootViewController = viewController;
	window.MakeKeyAndVisible ();

	Hub = new NotificationHub(
		"<notification hub name>",
		"<DefaultListenSharedAccessSignature connection string>");

	return true;
}
```

###Android
One way to instantiate a singleton instance is to store it as a parameter in the constructor of the inherited **IntentService** class, as shown here:

```csharp
using ByteSmith.WindowsAzure.Messaging;
...

[Service]
public class RemoteNotificationService : IntentService
{
	public static INotificationHub Hub { get; set; }

	static RemoteNotificationService ()
	{
		Hub = new NotificationHub(
			"<notification hub name>",
			"<DefaultListenSharedAccessSignature connection string>");
	}
		
	...
}
```

##Register for Native Notifications
One way to send push notifications is to have the backend specify the platform-specific payload of the notification, called native notifications. Your client app registers for native notifications by creating a native registration in the notification hub. This approach makes your client app responsible only for creating this registration in the notification hub, and keeping it up-to-date with the latest Token/Registration ID and tags. 

The benefits of this approach are a simple client-side configuration, and full control of the format of the notifications in the backend. The disadvantages are the need to create and send multiple platform-specific payloads from your back-end app, and a more complex implementation of per-user personalization of notifications. 

###iOS

```csharp
using ByteSmith.WindowsAzure.Messaging;
...

public async override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
{
	try {
		var registration = await Hub.RegisterNativeAsync(deviceToken);
		Debug.WriteLine("Native Registration ID: {0}", registration.RegistrationId);
	} catch (Exception ex) {
		// handle error
	}
}

public override void FailedToRegisterForRemoteNotifications (UIApplication application , NSError error)
{
	// handle error
}
```

###Android

```csharp
using ByteSmith.WindowsAzure.Messaging;
...

protected async void HandleRegistration(Context context, Intent intent)
{
	try {
		string registrationId = intent.GetStringExtra("registration_id");
		string error = intent.GetStringExtra("error");
		
		if (!String.IsNullOrEmpty (registrationId)) {
			var registration = await Hub.RegisterNativeAsync(registrationId);
			Debug.WriteLine("Native Registration ID: {0}", registration.RegistrationId);
		else if (!String.IsNullOrEmpty (error)) {
			// handle error
		}
	} catch (Exception ex) {
		// handle error
	}
}
```