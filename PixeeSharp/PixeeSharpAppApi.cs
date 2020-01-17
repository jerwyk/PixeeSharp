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

        /// <summary>
        /// Send a request to a specific url
        /// </summary>
        /// <param name="url">The url to request from</param>
        /// <returns>The response as a string</returns>
        public override async Task<string> GetUriResult(Uri url)
        {
            return await GetStringRequest(Method.GET, url, requireAuth: true).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the detai of a pixiv user
        /// </summary>
        /// <param name="id">The user's id, if not specified it will return the detail of the current login user</param>
        /// <returns></returns>
        public async Task<PixivUserProfile> GetUserDetail(string id = null, bool requireAuth = true)
        {
            id = id ?? UserID;
            Uri url = new Uri(baseUrl + "/v1/user/detail");
            PixivRequestContent _query = new PixivRequestContent();
            _query.Add("user_id", id);
            _query.Add("filter", Filter);
            string resJson = await GetStringRequest(Method.GET, url, query: _query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivUserProfile.Parse(resJson, this);
        }

        /// <summary>
        /// Get the details of a pixiv illustration
        /// </summary>
        /// <param name="id">The id of the illustration</param>
        /// <returns>A PixivIllustration object that contains the illustration information</returns>
        public async Task<PixivIllustration> GetIllustrationDetail(string id, bool requireAuth = true)
        {
            Uri url = new Uri(baseUrl + "/v1/illust/detail");
            PixivRequestContent _query = new PixivRequestContent();
            _query.Add("illust_id", id);
            string resJson = await GetStringRequest(Method.GET, url, query: _query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivIllustration.Parse(resJson, this, true);
        }

        /// <summary>
        /// Search for illustrations on pixiv
        /// </summary>
        /// <param name="word">The words to search</param>
        /// <param name="searchTarget">The mode to search with, the options are: partial_match_for_tags, exact_match_for_tags, title_and_caption</param>
        /// <param name="sort">The sorting order of the results, can be: date_desc, date_asc</param>
        /// <param name="duration">The search durations: within_last_day, within_last_week, within_last_month</param>
        /// <param name="offset">The offset of the search result</param>
        /// <returns>A PixivResult object that contains the searched illustration</returns>
        public async Task<PixivResult<PixivIllustration>> SearchIllustration(string word, string searchTarget = "partial_match_for_tags",
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
            return PixivResult<PixivIllustration>.Parse(resJson, "illusts",this);
        }

        /// <summary>
        /// Get the illustrations on the pixiv ranking
        /// </summary>
        /// <param name="mode">The mode of the ranking: day, week, month, day_male, day_female, week_original, week_rookie, day_manga</param>
        /// <param name="date">The date of the ranking</param>
        /// <param name="offset">The offset of the result</param>
        /// <returns>A PixivResult object that contains the ranking illustrations</returns>
        public async Task<PixivResult<PixivIllustration>> GetIllustrationRanking(string mode = "day", DateTime? date = null, int offset = -1, bool requireAuth = true)
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
            return PixivResult<PixivIllustration>.Parse(resJson, "illusts", this);
        }

        /// <summary>
        /// Get the trending tags on pixiv with a illustration attached to each tag
        /// </summary>
        /// <returns>A PixivTrendingTags that contains the current trending tags</returns>
        public async Task<PixivTrendingTags> GetTrendingTags(bool requireAuth = true)
        {
            Uri url = new Uri(baseUrl + "/v1/trending-tags/illust");
            var resJson = await GetStringRequest(Method.GET, url, query: new PixivRequestContent(("filter", Filter)), requireAuth: requireAuth).ConfigureAwait(false);
            return PixivTrendingTags.Parse(resJson, this);
        }

        /// <summary>
        /// Get the uploaded illustrations from a specific user
        /// </summary>
        /// <param name="id">The id of the user, if not specified it will get for the current login user/param>
        /// <param name="type">The type of the work to search for: illust, manga</param>
        /// <param name="offset">The offset of the result</param>
        /// <returns>A PixivResult object that contains the illustrations</returns>
        public async Task<PixivResult<PixivIllustration>> GetUserIllustrations(string id = null, string type = "illust", int offset = -1, bool requireAuth = true)
        {
            id = id ?? UserID;
            Uri url = new Uri(baseUrl + "/v1/user/illusts");
            PixivRequestContent query = new PixivRequestContent
            (
                ("user_id", id),
                ("filter", Filter)
            );
            query.Add("type", type);
            if (offset >= 0) query.Add("offset", offset.ToString());
            var resJson = await GetStringRequest(Method.GET, url, query: query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivResult<PixivIllustration>.Parse(resJson, "illusts", this);
        }

        /// <summary>
        /// Get the illustration that a user bookmarked
        /// </summary>
        /// <param name="id">The id of the user, if not specified it will be the current login user</param>
        /// <param name="restrict">The restriction type of the bookmarks: public, private</param>
        /// <param name="maxBookmarkId">The max bookmark id</param>
        /// <param name="tag">The tag of the bookmarks</param>
        /// <returns>A PixivResult object that contains the bookmarked illustrations</returns>
        public async Task<PixivResult<PixivIllustration>> GetUserBookmarksIllust(string id = null, string restrict = "public", string maxBookmarkId = null, string tag = null, bool requireAuth = true)
        {
            id = id ?? UserID;
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
            return PixivResult<PixivIllustration>.Parse(resJson, "illusts", this);
        }

        public async Task<PixivResult<PixivIllustration>> GetFollowIllutration(string restrict = "public", int offset = -1, bool requireAuth = true)
        {
            Uri url = new Uri(baseUrl + "/v2/illust/follow");
            PixivRequestContent query = new PixivRequestContent(("restrict", restrict));
            if (offset >= 0) query.Add("offset", offset.ToString());
            var resJson = await GetStringRequest(Method.GET, url, query: query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivResult<PixivIllustration>.Parse(resJson, "illusts", this);
        }
        
        //TODO: make a class for this
        /// <summary>
        /// Get the bookmark details of the specific illustration
        /// </summary>
        /// <param name="illustId">The id of the illustration</param>
        /// <returns>The response as raw json string</returns>
        public async Task<string> GetIllustrationBookmarkDetail(string illustId, bool requireAuth = true)
        {
            Uri url = new Uri(baseUrl + "/v2/illust/bookmark/detail");
            PixivRequestContent query = new PixivRequestContent(("illust_id", illustId));
            var resJson = await GetStringRequest(Method.GET, url, query: query, requireAuth: requireAuth).ConfigureAwait(false);
            return resJson;
        }

        /// <summary>
        /// Get the recommend works on the app front page
        /// </summary>
        /// <param name="contentType">The type of work to get: illust, manga</param>
        /// <param name="includeRankingLabel">If the result includes ranking labels</param>
        /// <param name="maxBookmarkIdForRecommend">Not sure what this does</param>
        /// <param name="minBookmarkIdForRecentIllust">Not sure what this does</param>
        /// <param name="offset">The offsef of the result</param>
        /// <param name="includeRankingIllusts">If the result includes ranking illustrations</param>
        /// <param name="BookmarkIllustIds">Not sure what this does</param>
        /// <param name="includePrivacyPolicy">If the result includes the pixiv pricate policy</param>
        /// <returns>A PixivRecommentIllustrationResult object that contains the results</returns>
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

        /// <summary>
        /// Get the related illustrations for a specific illustration
        /// </summary>
        /// <param name="id">The id of the illustration</param>
        /// <param name="seedIllustIds">Not sure what this does</param>
        /// <returns>A PixivResult object that contains the related illustrations</returns>
        public async Task<PixivResult<PixivIllustration>> GetRelatedIllustration(string id, List<string> seedIllustIds = null, bool requireAuth = true)
        {
            Uri url = new Uri(baseUrl + "/v2/illust/related");
            PixivRequestContent query = new PixivRequestContent
            (
                ("illust_id", id),
                ("filter", Filter)
            );
            foreach (var seed in seedIllustIds ?? Enumerable.Empty<string>())
            {
                query.Add("seed_illust_ids[]", seed);
            }
            string resJson = await GetStringRequest(Method.GET, url, query: query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivResult<PixivIllustration>.Parse(resJson, "illusts", this);
        }

        /// <summary>
        /// Get the illustraions that are associated with the users bookmarked tags
        /// </summary>
        /// <param name="id">The id of the user, if not specified it will be the current login user</param>
        /// <param name="restrict">The restriction of the tags</param>
        /// <param name="offset">The offset of the results</param>
        /// <returns>A PixivResult object that contains the bookmark tag illustrations</returns>
        public async Task<PixivResult<BookmarkTag>> GetUserBookmarkTagsIllust(string id = null, string restrict = "public", int offset = -1, bool requireAuth = true)
        {
            id = id ?? UserID;
            Uri url = new Uri(baseUrl + "/v1/user/bookmark-tags/illust");
            PixivRequestContent query = new PixivRequestContent
            (
                ("restrict", restrict),
                ("user_id", id)
            );
            if (offset >= 0) query.Add("offset", offset.ToString());
            var resJson = await GetStringRequest(Method.GET, url, query: query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivResult<BookmarkTag>.Parse(resJson, "bookmark_tags", this);
        }

        /// <summary>
        /// Add a illustration to the current login user's bookmarks
        /// </summary>
        /// <param name="id">The id of the illsutration</param>
        /// <param name="restrict">The restriction of the bookmark</param>
        /// <param name="tags">The tags that will be associated with this bookmark</param>
        public async Task AddBookmark(string id, string restrict = "public", List<string> tags = null, bool requireAuth = true)
        {
            Uri url = new Uri(baseUrl + "/v2/illust/bookmark/add");
            PixivRequestContent body = new PixivRequestContent
            (
                ("restrict", restrict),
                ("illust_id", id)
            );
            body.Add("tags", tags);
            await GetStringRequest(Method.POST, url, body: body, requireAuth: requireAuth).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a illustration from the current login user's bookmarks
        /// </summary>
        /// <param name="id">The id of the illustration</param>
        /// <returns>True if the optration is successful, false if it is not, which i likely because that the illustration is not bookmarked</returns>
        public async Task<bool> DeleteBookmark(string id, bool requireAuth = true)
        {
            try
            {
                Uri url = new Uri(baseUrl + "/v1/illust/bookmark/delete");
                PixivRequestContent body = new PixivRequestContent
                (
                    ("illust_id", id)
                );
                await GetStringRequest(Method.POST, url, body: body, requireAuth: requireAuth).ConfigureAwait(false);
                return true;
            }
            catch(PixivException ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Get a list of users which a user is following
        /// </summary>
        /// <param name="id">The id of the user, if not specified it will be the current login user</param>
        /// <param name="restrict">The restriction mode of the following users</param>
        /// <param name="offset">The offset of the result</param>
        /// <returns>A PixivResult object that contains the following users</returns>
        public async Task<PixivResult<PixivUserPreview>> GetUserFollowing(string id = null, string restrict = "public", int offset = -1, bool requireAuth = true)
        {
            id = id ?? UserID;
            Uri url = new Uri(baseUrl + "/v1/user/following");
            PixivRequestContent query = new PixivRequestContent
            (
                ("restrict", restrict),
                ("user_id", id)
            );
            if (offset >= 0) query.Add("offset", offset.ToString());
            var resJson = await GetStringRequest(Method.GET, url, query: query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivResult<PixivUserPreview>.Parse(resJson, "user_previews", this);
        }

        /// <summary>
        /// Get the followers of a specific user
        /// </summary>
        /// <param name="id">The id of the user, if not specified it will be the current login user</param>
        /// <param name="restrict">The restriction of the followers</param>
        /// <param name="offset">The offset of the result</param>
        /// <returns>A PixivResult object that contains the followers</returns>
        public async Task<PixivResult<PixivUserPreview>> GetUserFollower(string id = null, string restrict = "public", int offset = -1, bool requireAuth = true)
        {
            id = id ?? UserID;
            Uri url = new Uri(baseUrl + "/v1/user/follower");
            PixivRequestContent query = new PixivRequestContent
            (
                ("restrict", restrict),
                ("user_id", id)
            );
            if (offset >= 0) query.Add("offset", offset.ToString());
            var resJson = await GetStringRequest(Method.GET, url, query: query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivResult<PixivUserPreview>.Parse(resJson, "user_previews", this);
        }

        /// <summary>
        /// Get the MyPixiv (friend) of a certain user
        /// </summary>
        /// <param name="id">The id of the user, if not specified it will be the current login user</param>
        /// <param name="restrict">The restriction of the MyPixiv</param>
        /// <param name="offset">The offset of the results</param>
        /// <returns>A PixivResult object that contains the user previews</returns>
        public async Task<PixivResult<PixivUserPreview>> GetUserMyPixiv(string id = null, string restrict = "public", int offset = -1, bool requireAuth = true)
        {
            id = id ?? UserID;
            Uri url = new Uri(baseUrl + "/v1/user/mypixiv");
            PixivRequestContent query = new PixivRequestContent
            (
                ("restrict", restrict),
                ("user_id", id)
            );
            if (offset >= 0) query.Add("offset", offset.ToString());
            var resJson = await GetStringRequest(Method.GET, url, query: query, requireAuth: requireAuth).ConfigureAwait(false);
            return PixivResult<PixivUserPreview>.Parse(resJson, "user_previews", this);
        }

    }
}
