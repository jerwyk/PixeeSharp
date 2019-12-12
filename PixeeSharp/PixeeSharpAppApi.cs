using PixeeSharp.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeeSharp
{
    public class PixeeSharpAppApi : PixeeSharpBaseApi
    {

        public PixeeSharpAppApi() : base() { }

        public PixeeSharpAppApi(string AccessToken, string RefreshToken, string UserID, int RefreshInterval = 45) :
            base(AccessToken, RefreshToken, UserID, RefreshInterval)
        { }

        public PixeeSharpAppApi(PixeeSharpBaseApi BaseAPI) : base(BaseAPI) { }

        internal async Task<string> GetStringRequest(RestSharp.Method method, Uri url,
            PixivRequestHeader headers = null, PixivRequestContent query = null,
            PixivRequestContent body = null, bool requireAuth = true)
        {
            headers = headers ?? new PixivRequestHeader();
            if (!(headers.ContainsKey("User-Agent") || headers.ContainsKey("user-agent")))
            {
                headers.Add("App-OS", "ios");
                headers.Add("App-OS-Version", "10.3.1");
                headers.Add("App-Version", "6.7.1");
                headers.Add("User-Agent", "PixivIOSApp/6.7.1 (iOS 10.3.1; iPhone8,1)");
            }

            if (requireAuth)
                headers.Add("Authorization", $"Bearer {AccessToken}");

            return await base.GetStringRequest(method, url, headers, query, body).ConfigureAwait(false);

        }

        public async Task GetUserDetial()
        {

        }

        public async Task<PixivIllustration> GetIllustrationDetail(string id, bool requireAuth = true)
        {
            Uri url = new Uri("https://app-api.pixiv.net/v1/illust/detail");
            PixivRequestContent _query = new PixivRequestContent();
            _query.Add("illust_id", id);
            string resJson = await GetStringRequest(Method.GET, url, query: _query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivIllustration.GetIllustrationFromJson(resJson);
        }

        public async Task<PixivSearchResult> SearchIllust(string word, string searchTarget = "partial_match_for_tags",
            string sort = "date_desc", string duration = null, string filter = "for_ios", string offset = null,
            bool requireAuth = true)
        {
                Uri baseUrl = new Uri("https://app-api.pixiv.net/v1/search/illust");
                PixivRequestContent query = new PixivRequestContent
                (
                    ("word", word),
                    ("search_target", searchTarget),
                    ("sort", sort),
                    ("filter", filter)
                );
                if (!string.IsNullOrEmpty(duration)) query.Add("duration", duration);
                if (!string.IsNullOrEmpty(offset)) query.Add("offset", offset);
                var resJson = await GetStringRequest(Method.GET, baseUrl, query: query, requireAuth: requireAuth).ConfigureAwait(false);
                return PixivSearchResult.GetResultFromJson(resJson, this);
        }

    }
}
