using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeeSharp.Models
{
    public struct PixivProfile
    {
        public string Webpage { get; set; }
        public string Gender { get; set; }
        public string Birth { get; set; }
        public string BirthDay { get; set; }
        public int BirthYear { get; set; }
        public string Region { get; set; }
        public int AddressId { get; set; }
        public string CountryCode { get; set; }
        public string Job { get; set; }
        public int JobId { get; set; }
        public int TotalFollowUsers { get; set; }
        public int TotalMypixivUsers { get; set; }
        public int TotalIllusts { get; set; }
        public int TotalManga { get; set; }
        public int TotalNovels { get; set; }
        public int TotalIllustBookmarksPublic { get; set; }
        public int TotalIllustSeries { get; set; }
        public string BackgroundImageUrl { get; set; }
        public string TwitterAccount { get; set; }
        public string TwitterUrl { get; set; }
        public string PawooUrl { get; set; }
        public bool IsPremium { get; set; }
        public bool IsUsingCustomProfileImage { get; set; }

    }

    public struct PixivProfilePublicity
    {

        public string Gender { get; set; }
        public string Region { get; set; }
        public string BirthDay { get; set; }
        public string BirthYear { get; set; }
        public string Job { get; set; }
        public bool Pawoo { get; set; }
    }

    public class PixivUserProfile
    {

        public PixivUser User { get; set; }

        public PixivProfile Profile { get; set; }

        public PixivProfilePublicity ProfilePublicity { get; set; }

        public Dictionary<string, string> Workspace { get; set; }

        public PixeeSharpBaseApi Client { get; set; }

        public static PixivUserProfile GetUserProfileFromJson(string json, PixeeSharpBaseApi client = null)
        {
            var result = JsonConvert.DeserializeObject<PixivUserProfile>(json, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            });

            result.Client = client;

            return result;

        }

    }
}
