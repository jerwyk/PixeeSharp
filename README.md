# PixeeSharp
A Pixiv API inspired by Pixivpy and PixivCS, implimented with .NET framwork. It serializes all responses into c# objects, and provdies utility methods to handel them, therefore there is no need to deal with the raw json response yourself.

# Important
Pixiv no longer supports login with username and password. The current work around is to use refresh_token instead. Please follow the instructions [here](https://gist.github.com/ZipFile/c9ebedb224406f4f11845ab700124362) to create a refresh_token (not access_token) and use api.Auth(TOKEN) to login instead. Note that the token will invalid in some period of time, so you either need to get a new one or run a program to refresh it periodically. The library will refresh the token for you if the client object is not destroyed.

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
// client.Login("id or email", "password"); no longer works
client.Auth("refresh_token");
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
