using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeeSharp.Models
{
    public class PixivPrivacyPolicy
    {
        public string Version { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
    }

    public class PixivRecommentIllustrationResult : PixivResult<PixivIllustration>
    {
        public bool ContestExists { get; set; }
        public List<PixivIllustration> RankingIllustrations { get; set; }
        public PixivPrivacyPolicy PrivacyPolicy { get; set; }

        public static PixivRecommentIllustrationResult Parse(string json, PixeeSharpBaseApi client)
        {
            var result = Parse(json, "illusts", client);

            if (client != null)
            {
                foreach (var illust in result.Result ?? Enumerable.Empty<PixivIllustration>())
                {
                    illust.Client = client;
                }
                foreach (var illust in result.RankingIllustrations ?? Enumerable.Empty<PixivIllustration>())
                {
                    illust.Client = client;
                }
            }

            return result;
        }

    }
}
