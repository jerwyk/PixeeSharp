using Newtonsoft.Json;
using PixeeSharp.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeeSharp
{
    public class PixeeSharpAppApi : PixeeSharpBaseApi
    {

        public string Filter { get; set; } = "for_ios";
        private string baseUrl = "https://app-api.pixiv.net";

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

        public async Task<PixivUserProfile> GetUserDetail(string userId, bool requireAuth = true)
        {
            Uri url = new Uri(baseUrl + "/v1/user/detail");
            PixivRequestContent _query = new PixivRequestContent();
            _query.Add("user_id", userId);
            _query.Add("filter", Filter);
            string resJson = await GetStringRequest(Method.GET, url, query: _query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivUserProfile.Parse(resJson, this);
        }

        public async Task<PixivIllustration> GetIllustrationDetail(string id, bool requireAuth = true)
        {
            Uri url = new Uri(baseUrl + "/v1/illust/detail");
            PixivRequestContent _query = new PixivRequestContent();
            _query.Add("illust_id", id);
            string resJson = await GetStringRequest(Method.GET, url, query: _query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivIllustration.Parse(resJson, this, true);
        }

        public async Task<PixivIllustrationResult> SearchIllustration(string word, string searchTarget = "partial_match_for_tags",
            string sort = "date_desc", string duration = null, int offset = -1, bool requireAuth = true)
        {
            Uri url = new Uri(baseUrl + "/v1/search/illust");
            PixivRequestContent query = new PixivRequestContent
            (
                ("word", word),
                ("search_target", searchTarget),
                ("sort", sort),
                ("filter", Filter)
            );
            query.Add("duration", duration);
            if (offset >= 0) query.Add("offset", offset.ToString());
            var resJson = await GetStringRequest(Method.GET, url, query: query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivIllustrationResult.Parse(resJson, this);
        }

        public async Task<PixivIllustrationResult> GetIllustrationRanking(string mode = "day", DateTime? date = null, int offset = -1, bool requireAuth = true)
        {
            Uri url = new Uri(baseUrl + "/v1/illust/ranking");
            PixivRequestContent query = new PixivRequestContent
            (
                ("mode", mode),
                ("filter", Filter)
            );
            query.Add("date", date?.ToString("yyyy-MM-dd"));
            if (offset >= 0) query.Add("offset", offset.ToString());
            var resJson = await GetStringRequest(Method.GET, url, query: query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivIllustrationResult.Parse(resJson, this);
        }

        public async Task<PixivTrendingTags> GetTrendingTags(bool requireAuth = true)
        {
            Uri url = new Uri(baseUrl + "/v1/trending-tags/illust");
            var resJson = await GetStringRequest(Method.GET, url, query: new PixivRequestContent(("filter", Filter)), requireAuth: requireAuth).ConfigureAwait(false);
            return PixivTrendingTags.Parse(resJson, this);
        }

        public async Task<PixivIllustrationResult> GetUserIllustrations(string id, string type = "illust", int offset = -1, bool requireAuth = true)
        {
            Uri url = new Uri(baseUrl + "/v1/user/illusts");
            PixivRequestContent query = new PixivRequestContent
            (
                ("user_id", id),
                ("filter", Filter)
            );
            query.Add("type", type);
            if (offset >= 0) query.Add("offset", offset.ToString());
            var resJson = await GetStringRequest(Method.GET, url, query: query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivIllustrationResult.Parse(resJson, this);
        }

        public async Task<PixivIllustrationResult> GetUserBookmarksIllust(string id, string restrict = "public", string maxBookmarkId = null, string tag = null, bool requireAuth = true)
        {
            Uri url = new Uri(baseUrl + "/v1/user/bookmarks/illust");
            PixivRequestContent query = new PixivRequestContent
            (
                ("user_id", id),
                ("restrict", restrict),
                ("filter", Filter)
            );
            query.Add("max_bookmark_id", maxBookmarkId);
            query.Add("tag", tag);
            var resJson = await GetStringRequest(Method.GET, url, query: query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivIllustrationResult.Parse(resJson, this);
        }

        public async Task<PixivIllustrationResult> GetFollowIllutration(string restrict = "public", int offset = -1, bool requireAuth = true)
        {
            Uri url = new Uri(baseUrl + "/v2/illust/follow");
            PixivRequestContent query = new PixivRequestContent(("restrict", restrict));
            if (offset >= 0) query.Add("offset", offset.ToString());
            var resJson = await GetStringRequest(Method.GET, url, query: query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivIllustrationResult.Parse(resJson, this);
        }
        //TODO
        public async Task<string> GetIllustrationBookmarkDetail(string illustId, bool requireAuth = true)
        {
            Uri url = new Uri(baseUrl + "/v2/illust/bookmark/detail");
            PixivRequestContent query = new PixivRequestContent(("illust_id", illustId));
            var resJson = await GetStringRequest(Method.GET, url, query: query, requireAuth: requireAuth).ConfigureAwait(false);
            return resJson;
        }

        public async Task<PixivRecommentIllustrationResult> GetRecommendedIllustration(string contentType="illust", bool includeRankingLabel = true, 
            string maxBookmarkIdForRecommend = null, string minBookmarkIdForRecentIllust = null, int offset = -1, 
            bool? includeRankingIllusts = null, List<string> BookmarkIllustIds = null, bool? includePrivacyPolicy = false,
            bool requireAuth = true)
        {
            Uri url = new Uri(baseUrl + (requireAuth ? "/v1/illust/recommended" : "/v1/illust/recommended-nologin"));
            PixivRequestContent query = new PixivRequestContent
            (
                ("content_type", contentType),
                ("include_ranking_label", includeRankingLabel ? "true" : "false"),
                ("filter", Filter)
            );
            query.Add("max_bookmark_id_for_recommend", maxBookmarkIdForRecommend);
            query.Add("min_bookmark_id_for_recent_illust", minBookmarkIdForRecentIllust);
            if (offset >= 0) query.Add("offset", offset.ToString());            
            query.Add("include_privacy_policy", includePrivacyPolicy.ToString().ToLower());
            if (!requireAuth)
            {
                query.Add("bookmark_illust_ids", BookmarkIllustIds, ',');
            }
            if (includeRankingIllusts != null)
            {
                query.Add("include_ranking_illusts", includeRankingIllusts.Value ? "true" : "false");
            }
            string resJson = await GetStringRequest(Method.GET, url, query: query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivRecommentIllustrationResult.Parse(resJson, this);
        }



    }
}
