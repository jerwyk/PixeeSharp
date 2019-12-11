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

namespace PixeeSharp
{

    public class PixivRequestHeader
    {
        private Dictionary<string, string> _headers = new Dictionary<string, string>();

        public void Add(string key, string value)
        {
            _headers.Add(key, value);
        }

        /// <summary>
        /// Add the headers to the client
        /// </summary>
        /// <param name="request"></param>
        public void AddHeaders(ref RestRequest request)
        {
            foreach(var header in _headers)
            {
                request?.AddHeader(header.Key, header.Value);
            }
        }

    }

    public class PixivRequestParameter
    {
        private List<(string key, string value)> _parameters = new List<(string, string)>();

        public void Add(string key, string value)
        {
            _parameters.Add((key, value));
        }

        /// <summary>
        /// Add the headers to the client
        /// </summary>
        /// <param name="request"></param>
        public void AddParameter(ref RestRequest request)
        {
            foreach (var header in _parameters)
            {
                request?.AddQueryParameter(header.key, header.value);
            }
        }

        public string GetQueryString()
        {
            var array = (from i in _parameters
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(i.key),
                         HttpUtility.UrlEncode(i.value)))
                .ToArray();
            return "?" + string.Join("&", array);
        }

    }

    public class PixivException : Exception
    {
        public PixivException() { }
        public PixivException(string msg) : base(msg) { }
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

        internal string clientID = "MOBrBDS8blbauoSck0ZfDbtuzpyT";
        internal string clientSecret = "lsACyCD94FhDUtGTXi3QzcFE2uU1hqtDaKeqrdwj";
        internal string hashSecret = "28c1fdd170a5204386cb1313c7077b34f83e4aaf4aa829ce78c231e05b0bae2c";

        public Dictionary<string, string> TargetIPs { get; set; } = new Dictionary<string, string>()
        {
            {"oauth.secure.pixiv.net","210.140.131.224" },
            {"www.pixiv.net","210.140.131.224" },
            {"app-api.pixiv.net","210.140.131.224" }
        };

        public Dictionary<string, string> TargetSubjects { get; set; } = new Dictionary<string, string>()
        {
            {"210.140.131.224","CN=*.pixiv.net, O=pixiv Inc., OU=Development department, L=Shibuya-ku, S=Tokyo, C=JP" },
            {"210.140.92.142","CN=*.pximg.net, OU=Domain Control Validated" }
        };
        public Dictionary<string, string> TargetSNs { get; set; } = new Dictionary<string, string>()
        {
            {"210.140.131.224","281941D074A6D4B07B72D729" },
            {"210.140.92.142","2387DB20E84EFCF82492545C" }
        };
        public Dictionary<string, string> TargetTPs { get; set; } = new Dictionary<string, string>()
        {
            {"210.140.131.224","352FCC13B920E12CD15F3875E52AEDB95B62972B" },
            {"210.140.92.142","F4A431620F42E4D10EB42621C6948E3CD5014FB0" }
        };

        public string AccessToken { get; internal set; }
        public string RefreshToken { get; internal set; }
        public string UserID { get; internal set; }
        public bool ExperimentalConnection { get; set; }

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

        DispatcherTimer refreshTimer = new DispatcherTimer();

        //自动刷新登录时执行
        public event EventHandler<RefreshEventArgs> TokenRefreshed;

        public PixeeSharpBaseApi(string AccessToken, string RefreshToken, string UserID,
            bool ExperimentalConnection = false, int RefreshInterval = 45)
        {
            this.AccessToken = AccessToken;
            this.RefreshToken = RefreshToken;
            this.UserID = UserID;
            this.ExperimentalConnection = ExperimentalConnection;
            this.RefreshInterval = RefreshInterval;
            refreshTimer.Interval = TimeSpan.FromMinutes(RefreshInterval);
            refreshTimer.Tick += RefreshTimer_Tick;
        }

        private async void RefreshTimer_Tick(object sender, object e)
        {
            //每隔一定的时间刷新登录
            try
            {
                _ = await Auth(RefreshToken);
            }
            catch
            {
                TokenRefreshed(this, new RefreshEventArgs(null, null, false));
                return;
            }
            TokenRefreshed(this, new RefreshEventArgs(AccessToken, RefreshToken, true));
        }

        public PixeeSharpBaseApi() : this(null, null, null) { }

        public PixeeSharpBaseApi(PixeeSharpBaseApi BaseAPI) :
            this(BaseAPI.AccessToken, BaseAPI.RefreshToken, BaseAPI.UserID, BaseAPI.ExperimentalConnection, BaseAPI.RefreshInterval)
        { }

        public async Task<IRestResponse> RequestCall(RestSharp.Method Method, string Url,
            PixivRequestHeader Headers = null, PixivRequestParameter Query = null,
            HttpContent Body = null)
        {
            try
            {

                string queryUrl = Url + Query?.GetQueryString();

                RestClient _client = new RestClient();
                RestRequest _request = new RestRequest(queryUrl);
                Headers.AddHeaders(ref _request);
                Query.AddParameter(ref _request);

                var taskCompletionSource = new TaskCompletionSource<IRestResponse>();
                _client.ExecuteAsync(_request, (response) => taskCompletionSource.SetResult(response));

                return await taskCompletionSource.Task.ConfigureAwait(false);

            }
            catch { }         

        }

    }
}
