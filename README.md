# PixeeSharp
A Pixiv API inspired by Pixivpy and PixivCS, implimented with .NET framwork. It serializes all responses into c# objects, and provdies utility methods to handel them, therefore there is no need to deal with the raw json response yourself.

# Api supported functions
* Login and periodic authentication
* Get illstration details
* Get user details
* Search illustration
* Get illustration ranking
* Get illustraion recommendations
* Add and delete bookmarks
* Get following and followers

# How to use
Start by creating an intance of the PixeeSharpAppApi Object:
```C#
var client = new PixeeSharpAppApi();
```
Then you need to login to Pixiv using your Pixiv id and password
```C#
client.Login("id or email", "password");
```
After this, the api will preform a periodic login by itself.
Now you can request illustration and user information using the authenticated client.
The api is fully written in async, so it will await every request it sends.
The api will always return in the form of an C# object.
For example, getting an illustration with a provided id:
```C#
PixivIllustration illust = client.GetIllustrationDetail("76194604");
```
Pixiv has a security measure for its image server, so you need to use the following method to download the image as a stream:
```C#
Stream img = illust.GetImage(ImageSize.Large);
```
