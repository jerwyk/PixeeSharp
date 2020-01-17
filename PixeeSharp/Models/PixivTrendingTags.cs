using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeeSharp.Models
{

    public class TrendingTag : Tag
    {
        [JsonProperty("illust")]
        public PixivIllustration Illustration { get; set; }
    }

    public class PixivTrendingTags : PixivBaseModel
    {
        [JsonProperty("trend_tags")]
        public List<TrendingTag> TrendingTags { get; set; }

        public static PixivTrendingTags Parse(string json, PixeeSharpBaseApi client = null)
        {
            var res = Parse<PixivTrendingTags>(json, client);
            foreach(var t in res.TrendingTags)
            {
                t.Illustration.Client = client;
            }
            return res;
        }

    }
}
