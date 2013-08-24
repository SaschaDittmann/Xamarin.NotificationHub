#Windows Azure Notification Hubs for Xamarin

This project reflects the current state of development of the Xamarin Component "[Azure Notification Hubs](http://components.xamarin.com/view/NotificationHubs/)".

##About Xamarin

[Xamarin](http://xamarin.com/) is a San Francisco, California based software company created in May 2011. The company was created by the engineers that created Mono, MonoTouch and Mono for Android which are cross-platform implementations of the Common Language Infrastructure (CLI) and Common Language Specifications (often called Microsoft .NET).

##About Notification Hubs

**[Notification Hubs](http://www.windowsazure.com/en-us/documentation/services/notification-hubs/) provide a highly scalable, cross-platform push notification infrastructure that enables you to either broadcast push notifications to millions of users at once or tailor notifications to individual users.**

On every mobile platform, push notifications are a critical element of any application. Push Notifications are simply the most immediate means through which to engage and empower your users. Building and maintaining the infrastructure for a push notification system capable of reaching millions of users within minutes, however, is far from simple. On your own, delivering millions of push notifications within minutes would require tens of virtual machines running in parallel. We created Notification Hubs to give developers an easy and reliable way to reach their users on any platform and from any connected application backend.

Use Notification Hubs to:

###Broadcast cross-platform push notifications to millions of devices in minutes
Notification Hubs supply a common API to send push notifications to a variety of mobile platforms, including Windows Store, Windows Phone, iOS and Android. You can choose to send platform-specific notifications or broadcast a single platform-agnostic notification to all users. A few lines of code gives you the power to reach either all devices on a single platform or all iOS, Android and Windows devices at once.

Notification Hubs send out push notifications to millions of users within minutes, not hours. That makes this service a particularly good partner for when speed matters most—such as with breaking news.

###Send notifications from any backend
Notification Hubs can be used with any connected application—whether it’s built on Virtual Machines, Cloud Services, Web Sites, or Mobile Services. This makes it easy to update any of your mobile apps right away and start engaging your users on their terms.

###Target content to specific user segments
With Notification Hubs, you not only have the ability to broadcast notifications to all your users at once (regardless of their mobile platform). You also have the ability to subscribe users to any number of tags when you register them with a Notification Hub. Those tags give you an easy way to define and target user segments based on activity, interests, location, etc. with a single API call. By using those tags effectively, you never have to store and mange device tokens or Ids in your app’s backend in order to route notifications to particular users.

###Use templates to tailor each user’s notifications
Templates provide a way for developers to specify the exact format of the notification that each user receives based on each of their preferences. By using templates, there is no need to store the localization settings for each of your customers or to create hundreds of tags. You just need to register the templates that specify the correct language with a Notification Hub and send a single message with all the localized content. Once your Notification Hub receives that single message, it will extract the correct localized message for each targeted user from the message.

###Achieve extreme scale
Notification Hubs are optimized for massive scale. With Notification Hubs, you can quickly scale to millions of devices and billions of push notifications without ever having to re-architect or shard your application. The Notification Hub you configure for a given application will automatically handle the pub/sub scale-out infrastructure necessary to scale your message to every active device with incredibly low latency. All it takes is one message from your connect app’s backend to the Notification Hub and millions of push notifications will be fired off to your users.

Notification Hubs are backed by a high availability service level agreement. To start sending push notifications to every user on every device, you will need a Windows Azure account. Sign up for the free trial [here](http://www.windowsazure.com/en-us/pricing/free-trial/).