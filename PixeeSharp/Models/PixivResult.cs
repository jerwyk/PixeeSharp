using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeeSharp.Models
{
    public class PixivResult<T> : PixivBaseModel where T : IPixivModel
    {
        public string NextUrl { get; set; }
        public List<T> Result { get; set; }
        public int SearchSpanLimit { get; set; }

        private string ResultName;

        public static PixivResult<T> Parse(string json, string resultName = null, PixeeSharpBaseApi client = null)
        {
            var result = Parse<PixivResult<T>>(resultName == null ? json : json.Replace(resultName, "result"), client);
            if (client != null)
            {
                foreach (var r in result.Result)
                {
                    r.Client = client;
                }
            }
            result.ResultName = resultName;
            return result;
        }

        /// <summary>
        /// Get the next page of illustrations and returns it
        /// </summary>
        /// <returns>The search result</returns>
        public async Task<PixivResult<T>> ReturnNextResult()
        {
            return PixivResult<T>.Parse(await Client.GetUriResult(new Uri(NextUrl)).ConfigureAwait(false), ResultName, this.Client);
        }

        /// <summary>
        /// Get the next page of illustrations and overwrites the current illustrations
        /// </summary>
        /// <returns>If the next page is not avaliable then return false, otherwise true</returns>
        public async Task<bool> GetNextResult()
        {
            var nextResult = await ReturnNextResult().ConfigureAwait(false);
            this.Result = nextResult.Result;
            this.NextUrl = nextResult.NextUrl;
            return NextUrl != null;
        }

        /// <summary>
        /// Get the next page of illustrations and adds to the current list
        /// </summary>
        /// <returns>If the next page is not avaliable then return false, otherwise true</returns>
        public async Task<bool> AppendNextResult()
        {
            var nextResult = await ReturnNextResult().ConfigureAwait(false);
            this.Result.AddRange(nextResult.Result);
            this.NextUrl = nextResult.NextUrl;
 
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
