using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Windows.Threading;
using RestSharp;
using Newtonsoft.Json.Linq;
using PixeeSharp.Models;

namespace PixeeSharp
{

    internal class PixivRequestHeader
    {
        private Dictionary<string, string> _headers = new Dictionary<string, string>();

        public void Add(string key, string value)
        {
            if (!(string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)))
            {
                _headers.Add(key, value);
            }
        }

        public PixivRequestHeader(params (string key, string value)[] header)
        {
            _headers = header.ToDictionary(h => h.key, h => h.value);
        }

        /// <summary>
        /// Add the headers to the client
        /// </summary>
        /// <param name="request"></param>
        public void AddHeaders(ref RestRequest request)
        {
            foreach (var header in _headers)
            {
                request?.AddHeader(header.Key, header.Value);
            }
        }

        public bool ContainsKey(string key)
        {
            return _headers.ContainsKey(key);
        }

    }

    internal class PixivRequestContent
    {
        private Dictionary<string, string> _content = new Dictionary<string, string>();

        public PixivRequestContent(params (string key, string value)[] param)
        {
            _content = param.ToDictionary(h => h.key, h => h.value);
        }

        public void Add(string key, string value)
        {
            if (!(string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)))
            {
                _content.Add(key, value);
            }
        }

        public void Add(string key, IEnumerable<string> value, char seperator = ' ')
        {
            if(!(string.IsNullOrEmpty(key) || value == null || !value.Any()))
            {
                string valueString = "";
                foreach(string str in value)
                {
                    valueString += str + seperator;
                }
                _content.Add(key, valueString.TrimEnd(seperator));
            }
        }

        /// <summary>
        /// Add the content to the client
        /// </summary>
        /// <param name="request"></param>
        public void AddContent(ref RestRequest request)
        {
            foreach (var c in _content)
            {
                request?.AddParameter(c.Key, c.Value);
            }
        }

        public void AddQuery(ref RestRequest request)
        {
            foreach (var c in _content)
            {
                request?.AddQueryParameter(c.Key, c.Value);
            }
        }

    }

    [Serializable]
    public class PixivException : Exception
    {
        public PixivException() { }
        public PixivException(string msg) : base(msg) { }

        public PixivException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class RefreshEventArgs : EventArgs
    {
        public string NewAccessToken { get; }
        public string NewRefreshToken { get; }
        public bool IsSuccessful { get; }

        public RefreshEventArgs(string NewAccessToken, string NewRefreshToken, bool IsSuccessful)
        {
            this.NewAccessToken = NewAccessToken;
            this.NewRefreshToken = NewRefreshToken;
            this.IsSuccessful = IsSuccessful;
        }
    }

    public class PixeeSharpBaseApi
    {

        //Client secrets from PixivPy
        internal string clientID = "MOBrBDS8blbauoSck0ZfDbtuzpyT";
        internal string clientSecret = "lsACyCD94FhDUtGTXi3QzcFE2uU1hqtDaKeqrdwj";
        internal string hashSecret = "28c1fdd170a5204386cb1313c7077b34f83e4aaf4aa829ce78c231e05b0bae2c";

        //Properties
        public string AccessToken { get; internal set; }
        public string RefreshToken { get; internal set; }
        public string UserID { get; internal set; }

        private int refreshInterval;
        public int RefreshInterval
        {
            get => refreshInterval;
            set
            {
                refreshInterval = value;
                if (value > 0) refreshTimer.Interval = TimeSpan.FromMinutes(value);
            }
        }
        //Timer for auto regresh login
        DispatcherTimer refreshTimer = new DispatcherTimer();

        //Refresh Event
        public event EventHandler<RefreshEventArgs> TokenRefreshed;

        private async void RefreshTimer_Tick(object sender, object e)
        {
            //Auto login after a certain interval
            try
            {
                await Auth(RefreshToken).ConfigureAwait(false);
            }
            catch
            {
                TokenRefreshed(this, new RefreshEventArgs(null, null, false));
                return;
            }
            TokenRefreshed(this, new RefreshEventArgs(AccessToken, RefreshToken, true));
        }

        //Constructors
        public PixeeSharpBaseApi(string AccessToken, string RefreshToken, string UserID, int RefreshInterval = 45)
        {
            this.AccessToken = AccessToken;
            this.RefreshToken = RefreshToken;
            this.UserID = UserID;
            this.RefreshInterval = RefreshInterval;
            refreshTimer.Interval = TimeSpan.FromMinutes(RefreshInterval);
            refreshTimer.Tick += RefreshTimer_Tick;
        }
        public PixeeSharpBaseApi() : this(null, null, null) { }

        public PixeeSharpBaseApi(PixeeSharpBaseApi BaseAPI) :
            this(BaseAPI?.AccessToken, BaseAPI?.RefreshToken, BaseAPI?.UserID, BaseAPI.RefreshInterval)
        { }
        //-------------------------------------------------------

        /// <summary>
        /// Make a request to the specified url
        /// </summary>
        /// <param name="method">The HTTP method to use</param>
        /// <param name="url">The request url</param>
        /// <param name="headers">The request headers</param>
        /// <param name="query">The request query parameters</param>
        /// <param name="body">The request body content</param>
        /// <returns>The response of the request</returns>
        internal async Task<IRestResponse> RequestCall(RestSharp.Method method, Uri url,
            PixivRequestHeader headers = null, PixivRequestContent query = null,
            PixivRequestContent body = null)
        {
            try
            {

                RestClient _client = new RestClient(url);
                RestRequest _request = new RestRequest(method);
                headers?.AddHeaders(ref _request);
                body?.AddContent(ref _request);
                query?.AddQuery(ref _request);

                var taskCompletionSource = new TaskCompletionSource<IRestResponse>();
                _client.ExecuteAsync(_request, (response) => taskCompletionSource.SetResult(response));
                //return _client.Execute(_request);
                return await taskCompletionSource.Task.ConfigureAwait(false);

            }
            catch
            {
                throw new PixivException("Request failed");
            }

         }

        /// <summary>
        /// Excecute the request and returns the response string
        /// </summary>
        /// <param name="Method">The HTTP method to use</param>
        /// <param name="Url">The request url</param>
        /// <param name="Headers">The request headers</param>
        /// <param name="Query">The request query parameters</param>
        /// <param name="Body">The request body content</param>
        /// <returns>The response content of the request as a string</returns>
        internal async Task<string> GetStringRequest(Method method, Uri url,
           PixivRequestHeader headers = null, PixivRequestContent query = null,
           PixivRequestContent body = null)
        {
            IRestResponse res = await RequestCall(method, url, headers, query, body).ConfigureAwait(false);
            var status = res.StatusCode;
            if (!(status == HttpStatusCode.OK || status == HttpStatusCode.Moved || status == HttpStatusCode.Found))
                throw new PixivException("[ERROR] RequestCall() failed!");
            return res.Content;
        }

        /// <summary>
        /// Excecute the request and returns the content as stream
        /// </summary>
        /// <param name="method">The HTTP method to use</param>
        /// <param name="url">The request url</param>
        /// <param name="headers">The request headers</param>
        /// <param name="query">The request query parameters</param>
        /// <param name="body">The request body content</param>
        /// <returns>The response content of the request as a stream</returns>
        internal async Task<Stream> GetStreamRequest(RestSharp.Method method, Uri url,
           PixivRequestHeader headers = null, PixivRequestContent query = null,
           PixivRequestContent body = null)
        {
            IRestResponse res = await RequestCall(method, url, headers, query, body).ConfigureAwait(false);
            return new MemoryStream(res.RawBytes);
        }

        /// <summary>
        /// Login to Pixiv with a username and password
        /// </summary>
        /// <param name="username">The username of the account</param>
        /// <param name="password">The password of the account</param>
        public async Task Login(string username, string password)
        {
            string MD5Hash(string Input)
            {
                if (string.IsNullOrEmpty(Input)) return null;
                using (var md5 = MD5.Create())
                {
                    var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(Input.Trim()));
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                        builder.Append(bytes[i].ToString("x2"));
                    return builder.ToString();
                }
            }
            Uri url = new Uri("https://oauth.secure.pixiv.net/auth/token");
            string time = DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss") + "+00:00";

            PixivRequestHeader header = new PixivRequestHeader(
                ("User-Agent", "PixivAndroidApp/5.0.64 (Android 6.0)"),
                ("X-Client-Time", time),
                ("X-Client-Hash", MD5Hash(time + hashSecret))
            );

            PixivRequestContent content = new PixivRequestContent(
                ("get_secure_url", "1"),
                ("client_id", clientID),
                ("client_secret", clientSecret),
                ("grant_type", "password"),
                ("username", username),
                ("password", password)
            );

            dynamic resContent = JValue.Parse(await GetStringRequest(Method.POST, url, header, body: content).ConfigureAwait(false));
            AccessToken = resContent.response.access_token;
            UserID = resContent.response.user.id;
            RefreshToken = resContent.response.refresh_token;
            if (RefreshInterval > 0)
            {
                refreshTimer.Start();
            }

        }

        /// <summary>
        /// Login to Pixiv using a refresh token
        /// </summary>
        public async Task Auth(string refreshToken)
        {
            Uri url = new Uri("https://oauth.secure.pixiv.net/auth/token");
            PixivRequestHeader header = new PixivRequestHeader();
            header.Add("User-Agent", "PixivAndroidApp/5.0.64 (Android 6.0)");

            PixivRequestContent content = new PixivRequestContent
            (
                ("get_secure_url", "1"),
                ("client_id", clientID),
                ("client_secret", clientSecret),
                ("grant_type", "refresh_token"),
                ("refresh_token", refreshToken)
            );

            dynamic resContent = JValue.Parse(await GetStringRequest(Method.POST, url, header, body: content).ConfigureAwait(false));
            AccessToken = resContent.response.access_token;
            UserID = resContent.response.user.id;
            this.RefreshToken = resContent.response.refresh_token;
            if (RefreshInterval > 0)
            {
                refreshTimer.Start();
            }

        }

        public virtual async Task<string> GetUriResult(Uri url)
        {
            return await GetStringRequest(Method.GET, url).ConfigureAwait(false);
        }

        /// <summary>
        /// Download the image as stream from the Pixiv image server
        /// </summary>
        /// <param name="url">The url of the image</param>
        /// <returns>The image stream</returns>
        public async Task<Stream> DownloadImage(Uri url)
        {
            string referer = @"https://app-api.pixiv.net/";
            PixivRequestHeader header = new PixivRequestHeader();
            header.Add("Referer", referer);

            return await GetStreamRequest(Method.GET, url, header).ConfigureAwait(false);

        }

        /// <summary>
        /// Download the image as stream from the Pixiv image server
        /// </summary>
        /// <param name="url">The url of the image</param>
        /// <returns>The image stream</returns>
        public async Task<Stream> DownloadImage(string url) => await DownloadImage(new Uri(url)).ConfigureAwait(false);

    }
}
