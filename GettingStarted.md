#Azure Notification Hubs
##Getting Started
Push notification support in Windows Azure Service Bus enables you to access an easy-to-use, multiplatform, and scaled-out push infrastructure, which greatly simplifies the implementation of push notifications for both consumer and enterprise applications for mobile platforms.

##What are Push Notifications?
Smartphones and tablets have the ability to "notify" users when an event has occurred. On Windows Store applications, the notification can result in a toast: a modeless window appears, with a sound, to signal a new push. On Apple iOS devices, the push similarly interrupts the user with a dialog box to view or close the notification. Clicking View opens the application that is receiving the message.

Push notifications help mobile devices display fresh information while remaining energy-efficient. Push notifications are a vital component for consumer apps, where they are used to increase app engagement and usage. Notifications are also useful to enterprises, when up-to-date information increases employee responsiveness to business events.

Some specific examples of mobile engagement scenarios are:

1. Alerting a user with a toast that some work item has been assigned to that user, in a workflow-based enterprise app.
2. Displaying a badge with the number of current sales leads in a CRM app.

##How Push Notifications Work
Push notifications are delivered through platform-specific infrastructures called Platform Notification Systems (PNS). A PNS offers barebones functions (that is, no support for broadcast, personalization) and have no common interface. For instance, in order to send a notification to an iOS app, a developer must contact the APNS (Apple Push Notification Service), to send a notification to an Android device, the same developer has to contact GCM (Google Cloud Messaging), and send the message a second time.

At a high level, though, all platform notification systems follow the same pattern:

1. The client app contacts the PNS to retrieve its handle. The handle type depends on the system. For APNS, it is a token. For GCM, it is a Registration ID.
2. The client app stores this handle in the app back-end for later usage. For Apple, the system is called a provider.
3. To send a push notification, the app back-end contacts the PNS using the handle to target a specific client app instance.
4. The PNS forwards the notification to the device specified by the handle.

![image](http://www.windowsazure.com/media/devcenter/dotnet/sbpushnotifications1.gif)

##The Challenges of Push Notifications
While these systems are very powerful, they still leave much work to the app developer in order to implement even common push notification scenarios, like broadcast or push notification to a user.

Push notifications are one of the most requested features in cloud services for mobile apps. The reason is that the infrastructure required to make them work is fairly complex and mostly unrelated to the main business logic of the app. Some of the challenges in building an on demand push infrastructure are:

* **Platform dependency.** In order to send notifications to devices of different platforms, multiple interfaces must be coded in the back-end. Not only the low-level details are different but the presentation of the notification (tile, toast, or badge) is also platform-dependent which leads to complex and hard-to-maintain back-end code.

* **Scale.** Scaling this infrastructure has two aspects:

	* Per PNS guidelines, device tokens must be refreshed every time the app is launched. This leads to a large amount of traffic (and consequent database accesses) just to keep the device tokens up to date. When the number of devices grows (possibly to millions), the cost of creating and maintaining this infrastructure is nonnegligible.
	
	* Most PNS’ do not support broadcast to multiple devices. It follows that a broadcast to millions of devices results in millions of calls to the PNS’. Being able to scale these requests is nontrivial as usually app developers want to keep the total latency down (i.e. the last device to receive the message should not receive the notification 30 minutes after the notifications has been sent, as for many cases it would defeat the purpose to have push notifications).

* **Routing.** PNS’ provide a way to send a message to a device. In most apps though notifications are targeted to users and/or interest groups (i.e. all employees assigned to a certain customer account). It follows that the app back-end has to maintain a registry that associates interest groups with device tokens in order to route the notifications to the correct devices. This overhead adds to the total time to market and maintenance costs of an app.

##Why Use the Windows Azure Notification Hubs?
The Windows Azure Service Bus eliminates one great complexity: you do not have to manage the challenges of push notifications. Instead, use a Windows Azure Notification Hub.

Notification Hubs provide a ready-to-use push notification infrastructure that supports:

* **Multiple-platforms.** Notification Hubs provide a common interface to send notifications to all supported platforms. The app back-end can send notifications in platform-specific formats or in a platform-independent format. As of January 2013, Notification Hubs are able to push notifications to Windows Store apps and iOS apps. Support for Android and Windows Phone will be added soon.

* **Pub/Sub routing.** Each device, when sending its handle to a Notification Hub, can specify one or more tags. (See below for more details on tags.) Tags do not have to be pre-provisioned or disposed. Tags provide a simple way to send notifications to users/interest groups. Since tags can contain any app-specific identifier (such as user ids or group ids), their use frees the app back-end from the burden of having to store and manage device handles.

* **Scale.** Notification Hubs scale to millions of devices without the need of rearchitecting or sharding.

Notification Hubs use a full multiplatform, scaled-out push notification infrastructure, and considerably reduce the push-specific code that runs in the app backend. Notification Hubs implement all the functionalities of a push infrastructure. Devices are only responsible for registering their PNS handles, and the back-end responsible for sending platform independent messages to users or interest groups.

![image](http://www.windowsazure.com/media/devcenter/dotnet/sbpushnotifications2.gif)

##Create and configure a Notification Hub

###Prerequisites
Notification Hubs are backed by a high availability service level agreement. To start sending push notifications to every user on every device, you will need a Windows Azure account. Sign up for the free trial [here](http://www.windowsazure.com/en-us/pricing/free-trial/).

###Register your application with Windows Store
In order to send push notifications, your app must be registered with the appropriate PNS provider. 

You can follow the steps up to the section "Register your app for ..." in the **Getting Started with Notification Hubs** tutorials to register your app:

* [Get started with Notification Hubs - iOS](http://www.windowsazure.com/en-us/manage/services/notification-hubs/get-started-notification-hubs-ios/)
* [Get started with Notification Hubs - Android](http://www.windowsazure.com/en-us/manage/services/notification-hubs/get-started-notification-hubs-android/)

After completing these steps, you should have the required information, which you use later to configure your notification hub.

###Create a Namespace
A Service Bus namespace can contain multiple entities (queues, topics, relays, and notification hubs), and provides a root security context. You can create namespaces on the Windows Azure Management Portal. Once a namespace is created, entities can be created, updated, and deleted programmatically using the .NET Service Bus SDK listed in the “Prerequisites” section.

###Create and configure a Notification Hub
Using the Windows Azure Management Portal, you can create a notification hub and configure it to send push notifications to a Windows Store application.

1. Log on to the [Windows Azure Management Portal](http://manage.windowsazure.com/)
2. In the bottom left corner, click **New**.
3. Select **App Services**, then **Service Bus**, then **Notification Hub**, then **Quick Create**. 
4. Select a name for the notification hub, a region, and the namespace in which this notification hub will be created (if there are no namespaces available a new one with the desired name is provisioned).
5. Click the label “Create a new Notification Hub” at the bottom right of the screen.
6. Now, under the Service Bus tab on the left navigation pane, click the created namespace. The notification hub should appear in the list.
7. Click the notification hub, and then click the configure tab on the top.
8. Insert the information you received during registration in the appropriate section, and then click Save in the bottom toolbar.



 One way to instantiate a singleton instance is to store it as a parameter in the App class (defined in the file App.xaml.cs), as shown here:

##Client Application 

###Configure your app to use a Notification Hub

In order to register to a notification hub to receive push notifications, use the **NotificationHub** client class. 
This class must be a singleton in your client app.

####iOS
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

####Android
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

###Register for Native Notifications
One way to send push notifications is to have the backend specify the platform-specific payload of the notification, called native notifications. Your client app registers for native notifications by creating a native registration in the notification hub. This approach makes your client app responsible only for creating this registration in the notification hub, and keeping it up-to-date with the latest Token/Registration ID and tags. 

The benefits of this approach are a simple client-side configuration, and full control of the format of the notifications in the backend. The disadvantages are the need to create and send multiple platform-specific payloads from your back-end app, and a more complex implementation of per-user personalization of notifications. 

####iOS

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

####Android

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

###Register for Template Messages
####Template concepts

Templates enable a client application to specify the exact format of the notifications it wants to receive. Using templates, an app can achieve two main results: a platform-agnostic backend, and personalization of notifications.

The standard way to send push notifications is to send to platform notification services (WNS, APNS) a specific payload for each notification that is to be sent. For example, to send an alert to APNS, the payload is a json object of the following form:

```
{“aps”: {“alert” : “Hello!” }}
```

To send a similar toast message on a Windows Store application, the payload is as follows:

```
<toast>
  <visual>
    <binding template=\"ToastText01\">
      <text id=\"1\">Hello!</text>
    </binding>
  </visual>
</toast>
```

This requirement forces the app backend to produce different payloads for each platform, and effectively makes the backend responsible for part of the presentation layer of the app. Some concerns include localization and graphical layouts (especially for Windows Store apps that include notifications for various types of tiles).

The Notification Hubs template feature enables a client app to create special registrations, called template registrations, which include, in addition to the set of tags, a template. Given the preceding payload examples, the only platform-independent information is the actual alert message (Hello!). A template is basically a set of instructions for the Notification Hub on how to format a platform-independent message for the registration of that specific client app. In the preceding example, the platform independent message is a single property: message = **Hello!**

The template for an iOS client app registration is:

```
{“aps”:{“alert”:”$(message)”}}
```

The analogous template for a Windows Store client app is:

```
<toast>
  <visual>
    <binding template=\"ToastText01\">
      <text id=\"1\">$(message)</text>
    </binding>
  </visual>
</toast>
```

Note that the actual message is substituted for the expression $(message). This expression instructs the Notification Hub, whenever it sends a message to this particular registration, to build a message that follows this template.

Client applications can create multiple registrations in order to use multiple templates; for example, a template for alert messages and a template for tile updates, and to mix native registrations (registrations with no template) and template registrations.

> The Notification Hub sends one notification for each registration without considering whether they belong to the same client app. This behavior can be used to translate platform-independent notifications in more notifications. For example, the same platform independent message to the Notification Hub can be seamlessly translated in a toast alert and a tile update, without requiring the backend to be aware of it. Note that some platforms (for example, iOS) might collapse multiple notifications to the same device if they are sent in a short period of time.

####Expression Language.

The following table shows the language allowed in templates:

| Expression       | Description   |
| ---------------- | ------------- |
| $(prop)          | Reference to an event property with the given name. Property names are not case-sensitive. This expression resolves to the property’s text value or to an empty string if the property is not present. |
| $(prop, n)       | As above, but the text is explicitly clipped at n characters. For example, $(title, 20) clips the contents of the title property at 20 characters. |
| .(prop, n)       | As above, but the text is suffixed with three dots as it is clipped. The total size of the clipped string and the suffix does not exceed n characters. .(title, 20) with an input property of “This is the title line” results in This is the title…. |
| %(prop)          | Similar to $(name), except that the output is URI encoded. |
| #(prop)          | **Used in JSON templates.** <br/> This function works the same as $(prop) specified above, except when used in JSON templates (such as Apple templates). In this case, if this function: <br/><ul><li>is not surrounded by ‘{‘,’}’ (e.g. ‘myJsonProperty’ : ‘#(name)’), and</li><li>it evaluates to a number in Javascript format, i.e. regexp: (0|([1-9][0-9]*))(\.[0-9]+)?((e|E)(+|-)?[0-9]+)?,</li></ul> then the output JSON is a number. <br/> For example, ‘myJsonProperty’ : ‘#(name)’ becomes ‘myJsonProperty’ : 40 (and not ‘40‘). |
| 'text' or "text" | A literal. Literals contain arbitrary text enclosed in single or double quotes. |
| expr1 + expr2*   | The concatenation operator that joins two expressions into a single string. <br/> The expressions can be any of the preceding. <br/> When using concatenation, the entire expression must be surrounded with {}, e.g. {$(prop) + ‘ - ’ + $(prop2)}. |

####iOS


```csharp
using ByteSmith.WindowsAzure.Messaging;
...

public async override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
{
	try {
		const string template = "{\"aps\":{\"alert\":\"$(message)\"}}";
		var registration = await Hub.RegisterTemplateAsync(deviceToken, template, "toast");
		Debug.WriteLine("Template Registration ID: {0}", registration.RegistrationId);	} catch (Exception ex) {
		// handle error
	}
}

public override void FailedToRegisterForRemoteNotifications (UIApplication application , NSError error)
{
	// handle error
}
```

####Android

```csharp
using ByteSmith.WindowsAzure.Messaging;
...

protected async void HandleRegistration(Context context, Intent intent)
{
	try {
		string registrationId = intent.GetStringExtra("registration_id");
		string error = intent.GetStringExtra("error");
		
		if (!String.IsNullOrEmpty (registrationId)) {
			const string template = "{\"data\":{\"msg\":\"$(message)\"}}";
			var registration = await Hub.RegisterTemplateAsync(registrationId, template, "toast");
			Debug.WriteLine("Template Registration ID: {0}", registration.RegistrationId);
		else if (!String.IsNullOrEmpty (error)) {
			// handle error
		}
	} catch (Exception ex) {
		// handle error
	}
}
```

##Using tags to route notifications to sets of registrations

To target specific sets of registrations, the sender can send a message with a tag to a notification hub which then sends push notifications to all registrations with the specified tag.

The incoming message can contain a notification in a native format (WNS, APNS, GCM, or MPNS), or a platform-agnostic property bag (see the section on templates). Depending on the notification format, and the tags on the message, the notification is routed to a different set of registrations, as specified in the following table:

| Send message method | Message tag | Registrations with a template | Registrations with no template |
| ------------------- | ----------- | ----------------------------- | ------------------------------ |
| SendTemplateNotificationAsync | No tags | All | Never |
|  | Tag | All registrations with at least one specified tag. | Never |
| SendAppleNativeNotificationAsync | No tags | Never | All Apple registrations. |
|  | Tag | Never | All Apple registrations with at least one specified tag. |
| SendGcmNativeNotificationAsync | No tags | Never | All Android registrations. |
|  | Tag | Never | All Android registrations with at least one specified tag. |
| SendWindowsNativeNotificationAsync | No tags | Never | All Windows Store registrations. |
|  | Tag | Never | All Windows Store registrations with at least one specified tag. |
| SendMpnsNativeNotificationAsync | No tags | Never | All Windows Phone registrations. |
|  | Tag | Never | All Windows Phone registrations with at least one specified tag. |

###Use tags for interest groups
The following example uses an application from which you can receive toast notifications about specific bands. In this scenario, a simple way to route notifications is to label registrations with tags that represent the different bands, as in the following picture.

![image](http://i.msdn.microsoft.com/dynimg/IC640605.png)

In this picture, the message tagged **Beatles** reaches the two tablets with tags **beatles** and **Beatles, Wailers**.

Tags do not have to be pre-provisioned and can refer to multiple app-specific concepts. For example, assuming that users of this example application can comment on bands and want to receive toasts, not only for the comments on their favorite bands, but also for all comments from their friends, regardless of the band on which they are commenting. The following picture shows an example of this scenario.

![image](http://i.msdn.microsoft.com/dynimg/IC640606.png)

In this picture, Alice is interested in the Beatles, Bob is interested in the Wailers and in following Charlie’s comments, and Charlie is interested in the Wailers. Charlie is also interested in following Alice’s comments. When Charlie posts a comment on the Beatles, Alice receives the toast because of her interest in the Beatles, and Bob receives the toast because he is following Charlie.

###Use tags to target users
Another way to use tags is to identify all the devices of a particular user. Registrations can be tagged with a tag that contains a user id, as in the following picture.

![image](http://i.msdn.microsoft.com/dynimg/IC640607.png)

In this picture, the message tagged **uid:Alice** reaches all registrations tagged **uid:Alice**; hence, all Alice’s devices.

## Related Xamarin Tutorials

* [Push Notifications in iOS](http://docs.xamarin.com/guides/cross-platform/application_fundamentals/notifications/ios/remote_notifications_in_ios)
* [Push Notifications in Android](http://docs.xamarin.com/guides/cross-platform/application_fundamentals/notifications/android/remote_notifications_in_android)

## Related Microsoft Tutorials
* [Get started with Notification Hubs - iOS](http://www.windowsazure.com/en-us/manage/services/notification-hubs/get-started-notification-hubs-ios/)
* [Get started with Notification Hubs - Android](http://www.windowsazure.com/en-us/manage/services/notification-hubs/get-started-notification-hubs-android/)
* [Notify users with Notification Hubs - Mobile Services](http://www.windowsazure.com/en-us/manage/services/notification-hubs/notify-users/)
* [Notify users with Notification Hubs - ASP.NET](http://www.windowsazure.com/en-us/manage/services/notification-hubs/notify-users-aspnet/)
* [Send cross-platform notifications to users with Notification Hubs - Mobile Services](http://www.windowsazure.com/en-us/manage/services/notification-hubs/notify-users-xplat-mobile-services/)
* [Send cross-platform notifications to users with Notification Hubs - ASP.NET](http://www.windowsazure.com/en-us/manage/services/notification-hubs/notify-users-xplat-aspnet/)
* [Use Notification Hubs to send breaking news](http://www.windowsazure.com/en-us/manage/services/notification-hubs/breaking-news-dotnet/)
* [Use Notification Hubs to send localized breaking news](http://www.windowsazure.com/en-us/manage/services/notification-hubs/breaking-news-localized-dotnet/)
