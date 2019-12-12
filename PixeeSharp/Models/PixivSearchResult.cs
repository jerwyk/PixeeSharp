using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeeSharp.Models
{
    public class PixivSearchResult
    {
        [JsonProperty("illusts")]
        public List<PixivIllustration> Illustrations { get; set; }
        public string NextUrl { get; set; }
        public int SearchSpanLimit { get; set; }

        public PixeeSharpBaseApi Client { get; set; }

        public static PixivSearchResult GetResultFromJson(string json, PixeeSharpBaseApi client = null)
        {
            var result = JsonConvert.DeserializeObject<PixivSearchResult>(json, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            });

            if (client != null)
            {
                result.Client = client;
                foreach (var illust in result.Illustrations)
                {
                    illust.Client = client;
                }
            }

            return result;

        }

        /// <summary>
        /// Get the next page of illustrations and overwrites the current illustrations
        /// </summary>
        /// <returns>If the next page is not avaliable then return false, otherwise true</returns>
        public async Task<bool> GetNextResult()
        {
            var result = await ReturnNextResult().ConfigureAwait(false);
            this.Illustrations = result.Illustrations;
            this.NextUrl = result.NextUrl;
            this.SearchSpanLimit = result.SearchSpanLimit;
            return NextUrl != null;
        }

        /// <summary>
        /// Get the next page of illustrations and returns it
        /// </summary>
        /// <returns>The search result</returns>
        public async Task<PixivSearchResult> ReturnNextResult()
        {
            return PixivSearchResult.GetResultFromJson(await Client.GetUriResult(new Uri(NextUrl)).ConfigureAwait(false), this.Client);
        }

        /// <summary>
        /// Get the next page of illustrations and adds to the current list
        /// </summary>
        /// <returns>If the next page is not avaliable then return false, otherwise true</returns>
        public async Task<bool> AppendNextResult()
        {
            var result = await ReturnNextResult().ConfigureAwait(false);
            this.Illustrations.AddRange(result.Illustrations);
            this.NextUrl = result.NextUrl;
            this.SearchSpanLimit = result.SearchSpanLimit;
            return NextUrl != null;
        }

        /// <summary>
        /// Search until the end of the pages
        /// </summary>
        public async Task SearchAll()
        {
            bool ret = true;
            while (ret)
            {
                ret = await AppendNextResult().ConfigureAwait(false);
            }
        }

    }
}
